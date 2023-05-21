using Blazor.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using CSS = Blazor.Lists.Css.CssClass;

namespace Blazor.Lists;

public partial class ListBox<TItem> : ComponentControlBase, IAsyncDisposable
{
    #region Fields

    private TItem? _selectedItem;
    private int _selectedIndex = -1;

    // track request & wait for ui refresh
    private bool _scrollIntoViewRequired;

    private bool _isSelectionChanged;

    // cached reference
    private IList<TItem>? _items;

    // safe get count
    private int _itemsCount => _items?.Count ?? 0;

    private ElementReference? componentElement { get; set; }
    private ElementReference? SelectedItemElement { get; set; }
    
    private Lazy<Task<IJSObjectReference>>? _listModuleTask;
    private Lazy<Task<IJSObjectReference>>? _commonModuleTask;
    
    private DotNetObjectReference<ListBox<TItem>>? DotNetInstance;

    private const string  ListScriptFile = "./_content/Blazor.Lists/scripts/lists.js";

    #endregion

    #region Properties

    [Inject]
    public IJSRuntime? jsRuntime { get; set; }

    [Parameter]
    public string AriaLabel { get; set; } = "List";

    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public IEnumerable<TItem>? ItemSource { get; set; }

    [Parameter]
    public EventCallback<ListBoxEventArgs<TItem>> SelectionChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
#pragma warning disable BL0007
    public TItem SelectedItem
#pragma warning restore BL0007
    {
        get => _selectedItem!;
        set => _selectedItem = value;
    }

    [Parameter]
#pragma warning disable BL0007
    public int SelectedIndex
#pragma warning restore BL0007
    {
        get => _selectedIndex;
        set => _selectedIndex = value;
    }

    public EventCallback<int> SelectedIndexChanged { get; set; }


    #endregion

    #region Events

    private async Task ItemClickEventAsync(TItem item)
    {
        if (_selectedItem is not null && _selectedItem.Equals(item)) return;

        _selectedItem = item;
        _selectedIndex = _items!.IndexOf(item);

        await SelectionChanged.InvokeAsync(new ListBoxEventArgs<TItem>(this, _selectedItem)).ConfigureAwait(false);
    }

    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DotNetInstance = DotNetObjectReference.Create(this);
        _listModuleTask = new(() => jsRuntime!.ModuleFactory(ListScriptFile));
        _commonModuleTask = new(() => jsRuntime!.ModuleFactory(jsRuntime!.GetCommonScriptPath()));
    }

    protected override void OnParametersSet()
    {
        // make a reference to avoid excessive casting
        if (_items is null || _items.Count != ItemSource?.Count())
            _items = ItemSource?.ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            _scrollIntoViewRequired = false;
            _isSelectionChanged = false;
        }

        if (_scrollIntoViewRequired)
        {
            _scrollIntoViewRequired = false;
            await ScrollIntoView(SelectedItemElement!.Value).ConfigureAwait(false);
        }

        if (_isSelectionChanged && _selectedItem is not null)
        {
            _isSelectionChanged = false;
            await SelectionChanged.InvokeAsync(new ListBoxEventArgs<TItem>(this, _selectedItem!))
                .ConfigureAwait(false);
        }
    }

    #endregion

    #region Methods
   
    private string GetClass()
        => CssBuilder
            .Default(CSS.ListBox.Root)
            .AddClass(Class!, !string.IsNullOrWhiteSpace(Class))
            .Build();
 
    private string GetItemClass(TItem item)
        => CssBuilder
            .Default(CSS.ListBox.Item)
            .AddClass(CSS.ListBox.ItemModifier.Selected, IsSelectedState(item))
            .Build();

    private bool IsSelectedState(TItem item)
    {
        bool isSelected = _selectedItem is not null && _selectedItem.Equals(item);
        return isSelected;
    }

    private void SetSelectedIndex(int index)
    {
        if (_items is null || index >= _items.Count) return;

        _selectedIndex = index;
        _selectedItem = index < 0 ? default : _items[index];

        _isSelectionChanged = true;

        ScrollIntoView();
    }

    private void SetSelectedItem(TItem item)
    {
        if (_items is null || !_items.Contains(item)) return;

        _selectedItem = item;
        _selectedIndex = _items.IndexOf(item);

       _isSelectionChanged = true;

        ScrollIntoView();
    }

    private async Task FocusNextElement(bool isReverse = false)
    {
        await (await GetModuleInstance(true))
            .FocusNextElement(componentElement!.Value, isReverse)
            .ConfigureAwait(false);
    }

    private void ScrollIntoView()
    {
        _scrollIntoViewRequired = true;
        StateHasChanged();
    }

    private async Task ScrollIntoView(ElementReference? itemElement)
    {
        // now scroll the element into view
        await (await GetModuleInstance())
            .ScrollIntoViewAsync(itemElement!.Value)
            .ConfigureAwait(false);
    }

    private async Task<IJSObjectReference> GetModuleInstance(bool isCommonModule = false)
        => await (isCommonModule ? _commonModuleTask : _listModuleTask)!.Value;

    private async Task OnKeyDown(KeyboardEventArgs arg)
    {
        //Console.WriteLine($"** KEY: {arg.Code} | {arg.Key}");

        if (_itemsCount == 0 || _selectedIndex == -1)
            return;

        switch (arg.Key)
        {
            case "ArrowUp":
                MovePrevious();
                break;
            case "ArrowDown":
                MoveNext();
                break;
            case "Home":
                MoveFirst();
                break;
            case "End":
                MoveLast();
                break;
            case "Tab":
                await FocusNextElement(arg.ShiftKey);
                break;
        }
    }

    public void MovePrevious()
    {
        if (_selectedIndex < 1)
            return;
        
        SetSelectedIndex(SelectedIndex - 1);
    }

    public void MoveNext()
    {
        if (_selectedIndex >= _itemsCount)
            return;
        
        SetSelectedIndex(SelectedIndex + 1);
    }

    public void MoveFirst()
    {
        if (_itemsCount == 0)
            return;
        
        SetSelectedIndex(0);
    }

    public void MoveLast()
    {
        int count = _itemsCount;
        if (count == 0)
            return;

        SetSelectedIndex(count - 1);
    }

    public async ValueTask DisposeAsync()
    {
        if (_items is not null)
            _items = null;

        if (_listModuleTask!.IsValueCreated)
        {
            IJSObjectReference module = await _listModuleTask.Value;
            await module.DisposeAsync();
        }
    }

    #endregion
}