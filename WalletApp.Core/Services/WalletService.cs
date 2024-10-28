using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Entities;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Models;
using WalletApp.Core.Models.Requests;

namespace WalletApp.Core.Services;

public class WalletService : IWalletService
{
    private readonly IWalletAppDbContext _dbContext;

    public WalletService(IWalletAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet> CreateWallet(CancellationToken ct = default)
    {
        var data = new WalletEntity
        {
            Balance = 5000m,
            Currency = "EUR"
        };

        await _dbContext.Wallets.AddAsync(data, ct);
        await _dbContext.SaveAsync(ct);

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

        var conversionRate = await GetCurrencyConversionRate(wallet.Currency, currency, ct);

        return wallet.Balance * conversionRate;
    }

    public async Task<decimal> AdjustWalletBalance(AdjustWalletBalanceRequest request, CancellationToken ct = default)
    {
        //todo
        if (request.Strategy is null)
        {
            throw new WalletAppException("Undefined strategy");
        }

        var wallet = await _dbContext.Wallets.FindAsync([request.WalletId], ct)
            ?? throw new WalletAppException("Could not find wallet with ID " + request.WalletId);

        // If currency is unspecified, then assume wallet's currency.
        if (!string.IsNullOrEmpty(request.Currency))
        {
            request.Amount *= await GetCurrencyConversionRate(request.Currency, wallet.Currency, ct);
        }

        wallet.Balance = AdjustBalance(wallet.Balance, request.Amount, request.Strategy.Value);

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

    private async Task<decimal> GetCurrencyConversionRate(string currencyFrom, string currencyTo, CancellationToken ct = default)
    {
        var currentRates = await GetCurrentCurrencyRates(ct);

        if (currencyFrom == currencyTo)
        {
            return 1m;
        }

        if (currencyFrom == "EUR")
        {
            return currentRates.FirstOrDefault(x => x.Currency == currencyTo)?.Rate
                ?? throw new WalletAppException("Could not find currency rate for " + currencyTo);
        }

        var fromCurrencyRate = currentRates.FirstOrDefault(x => x.Currency == currencyFrom)?.Rate
            ?? throw new WalletAppException("Could not find currency rate for " + currencyFrom);

        if (currencyTo == "EUR")
        {
            return 1m / fromCurrencyRate;
        }

        var toCurrencyRate = currentRates.FirstOrDefault(x => x.Currency == currencyTo)?.Rate
            ?? throw new WalletAppException("Could not find currency rate for " + currencyTo);

        return toCurrencyRate / fromCurrencyRate;
    }

    private async Task<IEnumerable<CurrencyRateEntity>> GetCurrentCurrencyRates(CancellationToken ct = default)
    {
        return await _dbContext.CurrencyRates
            .Where(x => x.Date == _dbContext.CurrencyRates.Max(y => y.Date))
            .AsNoTracking()
            .ToArrayAsync(ct);
    }
}
