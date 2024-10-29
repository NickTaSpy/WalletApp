using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using WalletApp.Core.Interfaces;
using WalletApp.Infrastructure.Database;
using WalletApp.Infrastructure.Jobs;
using WalletApp.Infrastructure.Services;

namespace WalletApp.Infrastructure;

public static class WalletAppInfrastructureDIExtensions
{
    public static IServiceCollection AddWalletAppInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WalletAppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("WalletApp")));

        services.AddScoped<IWalletAppDbContext, WalletAppDbContext>();
        services.AddScoped<ICurrencyDataService, CurrencyDataService>();

        services.AddQuartz(q =>
        {
            q.AddJob<GetCurrenciesRatesJob>(opts => opts.WithIdentity(GetCurrenciesRatesJob.Key));

            q.AddTrigger(opts => opts
                .ForJob(GetCurrenciesRatesJob.Key)
                .WithIdentity(nameof(GetCurrenciesRatesJob) + "Trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever()));
        });

        services.AddQuartzServer(x => x.WaitForJobsToComplete = true);

        return services;
    }
}
