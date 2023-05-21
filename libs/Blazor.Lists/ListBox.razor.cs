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

    // cached reference
    private IList<TItem>? _items;

    private ElementReference? SelectedItemElement { get; set; }
    
    private Lazy<Task<IJSObjectReference>>? _moduleTask;
    
    private DotNetObjectReference<ListBox<TItem>>? DotNetInstance;

    private const string  ScriptFile = "./_content/Blazor.Lists/scripts/lists.js";

    private int _itemsCount => _items?.Count ?? 0;

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
        set => InvokeAsync(async () => await SetSelectedItemAsync(value));
    }

    [Parameter]
#pragma warning disable BL0007
    public int SelectedIndex
#pragma warning restore BL0007
    {
        get => _selectedIndex;
        set => InvokeAsync(async () => await SetSelectedIndexAsync(value));
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
        _moduleTask = new(() => jsRuntime!.ModuleFactory(ScriptFile));
    }

    protected override void OnParametersSet()
    {
        // make a reference to avoid excessive casting
        if (_items is null || _items.Count != ItemSource?.Count())
            _items = ItemSource?.ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_scrollIntoViewRequired)
        {
            _scrollIntoViewRequired = false;
            await ScrollIntoView(SelectedItemElement!.Value).ConfigureAwait(false);
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

    private async Task SetSelectedIndexAsync(int index)
    {
        if (_items is null || index >= _items.Count) return;

        _selectedIndex = index;
        _selectedItem = index < 0 ? default : _items[index];

        await ScrollIntoView().ConfigureAwait(false);
    }

    private async Task SetSelectedItemAsync(TItem item)
    {
        if (_items is null || !_items.Contains(item)) return;

        _selectedItem = item;
        _selectedIndex = _items.IndexOf(item);

        await ScrollIntoView().ConfigureAwait(false);
    }

    private bool _scrollIntoViewRequired;
    private Task ScrollIntoView()
    {
        //await ScrollIntoView(SelectedItemElement!.Value).ConfigureAwait(false);
        _scrollIntoViewRequired = true;
        return Task.CompletedTask;
    }

    public async Task ScrollIntoView(ElementReference? itemElement)
    {
        // now scroll the element into view
        await (await GetModuleInstance())
            .ScrollIntoViewAsync(itemElement!.Value)
            .ConfigureAwait(false);
    }

    private async Task<IJSObjectReference> GetModuleInstance()
        => await _moduleTask!.Value;

    private async Task OnKeyDown(KeyboardEventArgs arg)
    {
        //Console.WriteLine($"** KEY: {arg.Code} | {arg.Key}");

        if (_itemsCount == 0 || _selectedIndex == -1)
            return;

        switch (arg.Key)
        {
            case "ArrowUp":
                await MovePreviousAsync();
                break;
            case "ArrowDown":
                await MoveNextAsync();
                break;
            case "Home":
                await MoveFirstAsync();
                break;
            case "End":
                await MoveLastAsync();
                break;
            // future .. if we want to support manual selection, not select on navigation...
            //case "Space":
            //case "Enter":
            //    await this.SelectItemAsync(this._focusPanel);
            //    break;
        }
    }

    public async Task MovePreviousAsync()
    {
        if (_selectedIndex < 1) return;
        
        await SetSelectedIndexAsync(SelectedIndex - 1).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async Task MoveNextAsync()
    {
        if (_selectedIndex >= _itemsCount) return;
        
        await SetSelectedIndexAsync(SelectedIndex + 1).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async Task MoveFirstAsync()
    {
        if (_itemsCount == 0) return;
        
        await SetSelectedIndexAsync(0).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async Task MoveLastAsync()
    {
        int count = _itemsCount;
        if (count == 0) return;

        await SetSelectedIndexAsync(count - 1).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_items is not null)
            _items = null;

        if (_moduleTask!.IsValueCreated)
        {
            IJSObjectReference module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    #endregion
}