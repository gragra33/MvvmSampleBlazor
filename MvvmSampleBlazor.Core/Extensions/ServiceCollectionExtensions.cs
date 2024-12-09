﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmSample.Core.Services;
using MvvmSample.Core.ViewModels;
using MvvmSample.Core.ViewModels.Widgets;
using MvvmSampleBlazor.Services;

namespace MvvmSampleBlazor.Extensions;

public static class ServiceCollectionExtensions
{
    // obsolete - now uses the ViewModelDefinition attribute & auto registration
    //public static IServiceCollection AddViewModels(this IServiceCollection services)
    //{
    //    services.AddTransient<IIntroductionPageViewModel, SamplePageViewModel>();
    //    services.AddTransient<ObservableObjectPageViewModel>();
    //    services.AddTransient<RelayCommandPageViewModel>();
    //    services.AddTransient<AsyncRelayCommandPageViewModel>();
    //    services.AddTransient<MessengerPageViewModel>();
    //    services.AddTransient<MessengerSendPageViewModel>();
    //    services.AddTransient<MessengerRequestPageViewModel>();
    //    services.AddTransient<IocPageViewModel>();
    //    services.AddTransient<ISettingUpTheViewModelsPageViewModel, SamplePageViewModel>();
    //    services.AddTransient<ISettingsServicePageViewModel, SamplePageViewModel>();
    //    services.AddTransient<IRedditServicePageViewModel, SamplePageViewModel>();
    //    services.AddTransient<IBuildingTheUIPageViewModel, SamplePageViewModel>();
    //    services.AddTransient<IRedditBrowserPageViewModel, SamplePageViewModel>();

    //    services.AddTransient<ContactsListWidgetViewModel>();
    //    services.AddTransient<PostWidgetViewModel>();
    //    services.AddTransient<SubredditWidgetViewModel>();
    //    services.AddTransient<ValidationFormWidgetViewModel>();

    //    return services;
    //}

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IFilesService>(sp => new FilesService(sp.GetService<HttpClient>()));
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }
}