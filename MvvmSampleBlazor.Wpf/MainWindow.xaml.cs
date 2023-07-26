using MvvmSampleBlazor.Wpf.ViewModels;

namespace MvvmSampleBlazor.Wpf;

public partial class MainWindow
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
    }
}