using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

public class MessengerSendPageViewModel : MessengerPageViewModel
{
    public MessengerSendPageViewModel(IFilesService filesService) : base(filesService)
    {
        /* skip */
    }
}