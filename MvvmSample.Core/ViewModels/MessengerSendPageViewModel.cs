using Blazing.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public class MessengerSendPageViewModel : MessengerPageViewModel
{
    public MessengerSendPageViewModel(IFilesService filesService) : base(filesService)
    {
        /* skip */
    }
}