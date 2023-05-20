using Blazor.Common.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blazor.Common.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorPageHistory(this IServiceCollection services)
        => services.AddScoped<IPageHistoryService, PageHistoryService>();
 
    public static IServiceCollection AddBlazorPageHistory(this IServiceCollection services, int bufferCapacity)
    {
        services.TryAddScoped<IPageHistoryService>(builder =>
            new PageHistoryService(builder.GetService<NavigationManager>()!, bufferCapacity));

        return services;
    }
}