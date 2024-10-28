using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletApp.Core.Interfaces;
using WalletApp.Infrastructure.Database;

namespace WalletApp.Infrastructure;

public static class WalletAppInfrastructureDIExtensions
{
    public static IServiceCollection AddWalletAppInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WalletAppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("WalletApp")));

        services.AddScoped<IWalletAppDbContext, WalletAppDbContext>();
        return services;
    }
}
