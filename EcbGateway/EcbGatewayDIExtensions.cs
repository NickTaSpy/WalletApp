using EcbGateway.Interfaces;
using EcbGateway.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EcbGateway;

public static class EcbGatewayDIExtensions
{
    public static IServiceCollection AddEcbGateway(this IServiceCollection services)
    {
        //services.Configure<EcbGatewaySettings>((x =>
        //{
        //});

        services.AddHttpClient(Constants.EcbHttpClientName);
        services.TryAddScoped<IEcbService, EcbService>();
        return services;
    }
}
