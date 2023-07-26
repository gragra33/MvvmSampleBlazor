using System;

namespace MvvmSampleBlazor.Avalonia.ViewModels;

public record NavigationAction(string Title, Action Action)
{
}