using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

public class MessengerRequestPageViewModel : MessengerPageViewModel
{
    public MessengerRequestPageViewModel(IFilesService filesService) : base(filesService)
    {
        /* skip */
    }
}