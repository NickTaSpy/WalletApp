using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Models.Requests;
using WalletApp.Core.Services;
using WalletApp.Core.Validators;

namespace WalletApp.Core;

public static class WalletAppCoreDIExtensions
{
    public static IServiceCollection AddWalletAppCore(this IServiceCollection services)
    {
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IValidator<AdjustWalletBalanceRequest>, AdjustWalletBalanceRequestValidator>();
        return services;
    }
}
