using Blazing.Mvvm;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvvmSample.Core.Services;
using MvvmSampleBlazor.Extensions;
using MvvmSampleBlazor.Wpf.Extensions;
using Refit;

namespace MvvmSampleBlazor.Wpf;

public partial class App
{
    public App()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        IServiceCollection services = builder.Services;

        services.AddWpfBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        services
            .AddSingleton(RestService.For<IRedditService>("https://www.reddit.com/"))
            .AddViewModels()
            .AddServicesWpf()
            .AddMvvmNavigation();

#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

        services.AddScoped<MainWindow>();

        Resources.Add("services", services.BuildServiceProvider());

        // will throw an error
        //MainWindow = provider.GetRequiredService<MainWindow>();
        //MainWindow.Show();
    }
}