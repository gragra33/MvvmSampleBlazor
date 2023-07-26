using Blazing.Mvvm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MvvmSample.Core.Services;
using MvvmSampleBlazor;
using MvvmSampleBlazor.Extensions;
using Refit;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddSingleton(RestService.For<IRedditService>("https://www.reddit.com/"))
    .AddViewModels()
    .AddServices()
    .AddMvvmNavigation();

#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

await builder.Build().RunAsync();