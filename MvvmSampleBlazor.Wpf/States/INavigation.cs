using Blazing.Mvvm.ComponentModel;

namespace MvvmSampleBlazor.Wpf.States;

public interface INavigation
{
    void NavigateTo(string page);
    void NavigateTo<TViewModel>() where TViewModel : IViewModelBase;
}