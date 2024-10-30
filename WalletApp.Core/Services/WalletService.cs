using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Entities;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Models;
using WalletApp.Core.Models.Requests;
using WalletApp.Core.Validators;

namespace WalletApp.Core.Services;

public class WalletService : IWalletService
{
    private readonly IWalletAppDbContext _dbContext;
    private readonly IValidator<AdjustWalletBalanceRequest> _adjustWalletBalanceRequestValidator;
    private readonly IValidator<CreateWalletRequest> _createWalletRequestValidator;
    private readonly ILogger<WalletService> _logger;
    private readonly ICurrencyService _currencyService;

    public WalletService(
        IWalletAppDbContext dbContext,
        IValidator<AdjustWalletBalanceRequest> adjustWalletBalanceRequestValidator,
        IValidator<CreateWalletRequest> createWalletRequestValidator,
        ILogger<WalletService> logger,
        ICurrencyService currencyService)
    {
        _dbContext = dbContext;
        _adjustWalletBalanceRequestValidator = adjustWalletBalanceRequestValidator;
        _createWalletRequestValidator = createWalletRequestValidator;
        _logger = logger;
        _currencyService = currencyService;
    }

    public async Task<Wallet> CreateWallet(CreateWalletRequest request, CancellationToken ct = default)
    {
        _createWalletRequestValidator.ValidateAndThrow(request);

        var data = new WalletEntity
        {
            Balance = request.Balance,
            Currency = request.Currency,
        };

        await _dbContext.Wallets.AddAsync(data, ct);
        await _dbContext.SaveAsync(ct);

        _logger.LogInformation("Created new wallet: {@wallet}", data);

        return new Wallet
        {
            Id = data.Id,
            Balance = data.Balance,
            Currency = data.Currency,
        };
    }

    public async Task<decimal> RetrieveWalletBalance(long walletId, string? currency, CancellationToken ct = default)
    {
        var wallet = await _dbContext.Wallets.FindAsync([walletId], ct)
            ?? throw new WalletAppException("Could not find wallet with ID " + walletId);

        if (string.IsNullOrEmpty(currency) || wallet.Currency == currency)
        {
            return wallet.Balance;
        }

        var conversionRate = await _currencyService.GetCurrencyConversionRate(wallet.Currency, currency, ct);

        return wallet.Balance * conversionRate;
    }

    public async Task<decimal> AdjustWalletBalance(long walletId, AdjustWalletBalanceRequest request, CancellationToken ct = default)
    {
        _adjustWalletBalanceRequestValidator.ValidateAndThrow(request);

        var wallet = await _dbContext.Wallets.FindAsync([walletId], ct)
            ?? throw new WalletAppException("Could not find wallet with ID " + walletId);

        // If currency is unspecified, then assume wallet's currency.
        if (!string.IsNullOrEmpty(request.Currency))
        {
            request.Amount *= await _currencyService.GetCurrencyConversionRate(request.Currency, wallet.Currency, ct);
        }

        wallet.Balance = AdjustBalance(wallet.Balance, request.Amount, request.Strategy!.Value);

        await _dbContext.SaveAsync(ct);

        return wallet.Balance;
    }

    private static decimal AdjustBalance(decimal currentBalance, decimal amount, AdjustFundsStrategy strategy)
    {
        switch (strategy)
        {
            case AdjustFundsStrategy.AddFundsStrategy:
                return currentBalance + amount;
            case AdjustFundsStrategy.SubtractFundsStrategy:
                if (currentBalance < amount)
                {
                    throw new WalletAppException("Wallet lacks sufficient funds.");
                }

                return currentBalance - amount;
            case AdjustFundsStrategy.ForceSubtractFundsStrategy:
                return currentBalance - amount;
            default:
                throw new WalletAppException($"{nameof(AdjustFundsStrategy)}.{strategy} not supported.");
        }
    }
}
