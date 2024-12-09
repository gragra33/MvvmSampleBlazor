using Blazing.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Core.Services;

namespace MvvmSample.Core.ViewModels;

[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public class MessengerRequestPageViewModel : MessengerPageViewModel
{
    public MessengerRequestPageViewModel(IFilesService filesService) : base(filesService)
    {
        /* skip */
    }
}