using Blazing.Mvvm.ComponentModel;

namespace MvvmSampleBlazor.Avalonia.States;

public interface INavigation
{
    void NavigateTo(string page);
    void NavigateTo<TViewModel>() where TViewModel : IViewModelBase;
}