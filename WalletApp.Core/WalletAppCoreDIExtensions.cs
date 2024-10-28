using Microsoft.Extensions.DependencyInjection;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Services;

namespace WalletApp.Core;

public static class WalletAppCoreDIExtensions
{
    public static IServiceCollection AddWalletAppCore(this IServiceCollection services)
    {
        services.AddScoped<IWalletService, WalletService>();
        return services;
    }
}
