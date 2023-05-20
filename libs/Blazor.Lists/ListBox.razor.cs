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

    private ElementReference? SelectedItemElement { get; set; }
    
    private Lazy<Task<IJSObjectReference>>? _moduleTask;
    
    private DotNetObjectReference<ListBox<TItem>>? DotNetInstance;

    private const string  ScriptFile = "./_content/Blazor.Lists/scripts/lists.js";

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
    public TItem SelectedItem
    {
        get => _selectedItem!;
        set => InvokeAsync(async () => await SetSelectedItemAsync(value));
    }

    [Parameter]
    public int SelectedIndex
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
        _selectedIndex = ItemSource!.ToList().IndexOf(item);

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
        if (ItemSource is null || index >= ItemSource.ToList().Count) return;

        _selectedIndex = index;
        _selectedItem = index < 0 ? default : ItemSource.ToList()[index];

        await ScrollIntoView().ConfigureAwait(false);
    }

    private async Task SetSelectedItemAsync(TItem item)
    {
        if (ItemSource is null || !ItemSource.Contains(item)) return;

        _selectedItem = item;
        _selectedIndex = ItemSource.ToList().IndexOf(item);

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
        if (_selectedIndex >= ItemSource!.ToList().Count) return;
        await SetSelectedIndexAsync(SelectedIndex + 1).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async Task MoveFirstAsync()
    {
        if (ItemSource!.ToList().Count == 0) return;
        await SetSelectedIndexAsync(0).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async Task MoveLastAsync()
    {
        if (ItemSource!.ToList().Count == 0) return;
        await SetSelectedIndexAsync(ItemSource!.ToList().Count - 1).ConfigureAwait(false);
        await ScrollIntoView().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask!.IsValueCreated)
        {
            IJSObjectReference module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    #endregion
}