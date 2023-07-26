using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Core.Services;
using MvvmSampleBlazor.Services;
using FilesService = MvvmSampleBlazor.Avalonia.Services.FilesService;

namespace MvvmSampleBlazor.Avalonia.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServicesWpf(this IServiceCollection services)
    {
        services.AddTransient<IFilesService>(sp => new FilesService());
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }
}