﻿using Microsoft.AspNetCore.Components;

namespace Blazor.Common;

public abstract class ComponentControlBase : ComponentBase, IDisposable
{
    #region Fields
    
    #endregion

    #region Properties

    public string? UniqueId { get; private set; }

    [Parameter]
    public ElementReference Reference { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public virtual string? Class { get; set; }

    [Parameter]
    public virtual string? Style { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    public IDictionary<string, string> CurrentStyle
    {
        get
        {
            Dictionary<string, string> currentStyle = new();

            if (string.IsNullOrEmpty(Style)) return currentStyle;
            
            foreach (string pair in Style.Split(';'))
            {
                string[] keyAndValue = pair.Split(':');
                    
                if (keyAndValue.Length != 2) continue;
                    
                string key = keyAndValue[0].Trim();
                string value = keyAndValue[1].Trim();

                currentStyle[key] = value;
            }

            return currentStyle;
        }
    }
    
    #endregion

    #region Overrides
    
    protected override void OnInitialized()
    {
        UniqueId = GetUniqueId();
        base.OnInitialized();
    }

    #endregion

    #region Methods

    protected string? GetId()
        => AdditionalAttributes is not null
           && AdditionalAttributes.TryGetValue("id", out object? id)
           && !string.IsNullOrEmpty(Convert.ToString(id))
            ? $"{id}"
            : UniqueId;

    public string GetUniqueId()
        => Guid.NewGuid().ToIdString();

    public new void StateHasChanged()
        => InvokeAsync(base.StateHasChanged);

    public async Task StateHasChangedAsync()
        => await InvokeAsync(base.StateHasChanged).ConfigureAwait(false);

    public virtual string GetComponentCssClass() => Class ?? "";

    public virtual void Dispose()
    {
        //
    }

    #endregion
}