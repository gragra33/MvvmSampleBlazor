using System;

namespace MvvmSampleBlazor.Wpf.ViewModels;

public record NavigationAction(string Title, Action Action)
{
}