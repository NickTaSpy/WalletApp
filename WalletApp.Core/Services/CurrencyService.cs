using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using WalletApp.Core.Entities;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Models.Requests;
using Microsoft.EntityFrameworkCore;
using EcbGateway.Interfaces;

namespace WalletApp.Core.Services;

public class CurrencyService : ICurrencyService
{
    private const string CurrentCurrenciesRatesCacheKey = "CurrentCurrenciesRates";

    private readonly IWalletAppDbContext _dbContext;
    private readonly IValidator<AdjustWalletBalanceRequest> _adjustWalletBalanceRequestValidator;
    private readonly IMemoryCache _memoryCache;
    private readonly IEcbService _ecbService;
    private readonly ICurrencyDataService _currencyDataService;

    public CurrencyService(
        IWalletAppDbContext dbContext,
        IValidator<AdjustWalletBalanceRequest> adjustWalletBalanceRequestValidator,
        IMemoryCache memoryCache,
        IEcbService ecbService,
        ICurrencyDataService currencyDataService)
    {
        _dbContext = dbContext;
        _adjustWalletBalanceRequestValidator = adjustWalletBalanceRequestValidator;
        _memoryCache = memoryCache;
        _ecbService = ecbService;
        _currencyDataService = currencyDataService;
    }

    public async Task<decimal> GetCurrencyConversionRate(string currencyFrom, string currencyTo, CancellationToken ct = default)
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

    public async Task<IEnumerable<CurrencyRateEntity>> GetCurrentCurrencyRates(CancellationToken ct = default)
    {
        var currencyRates = await _memoryCache.GetOrCreateAsync(CurrentCurrenciesRatesCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            var maxDate = _dbContext.CurrencyRates.Max(y => y.Date);

            return await _dbContext.CurrencyRates
                .Where(x => x.Date == maxDate)
                .AsNoTracking()
                .ToArrayAsync(ct);
        });

        if (currencyRates is null)
        {
            throw new WalletAppException("Failed to get current currency rates.");
        }

        return currencyRates;
    }

    public async Task UpdateCurrencyRates(CancellationToken ct = default)
    {
        var currentCurrenciesRates = await _ecbService.GetCurrenciesRates(ct);

        await _currencyDataService.SaveOrUpdateCurrenciesRates(currentCurrenciesRates, ct);

        _memoryCache.Remove(CurrentCurrenciesRatesCacheKey);
    }
}
