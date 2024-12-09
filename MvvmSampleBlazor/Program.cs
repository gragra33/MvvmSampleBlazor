using Blazing.Mvvm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MvvmSample.Core.Services;
using MvvmSample.Core.ViewModels;
using MvvmSampleBlazor;
using MvvmSampleBlazor.Extensions;
using Refit;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddSingleton(RestService.For<IRedditService>("https://www.reddit.com/"))
    //.AddViewModels() // obsolete - now uses the ViewModelDefinition attribute & auto registration
    .AddServices()
    .AddMvvm(options =>
    { 
        options.RegisterViewModelsFromAssemblyContaining<SamplePageViewModel>();
    });

#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

await builder.Build().RunAsync();