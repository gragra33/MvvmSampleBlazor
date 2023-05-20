using Microsoft.AspNetCore.Components;

namespace Blazor.Common;

public interface IElementCallback
{
    Microsoft.AspNetCore.Components.EventCallback Execute { get; }
}

public interface IElementCallback<T>
{
    EventCallback<T> Execute { get; }
}