using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using WalletApp.Core.Entities;

namespace WalletApp.Core.Interfaces;

public interface ICurrencyService
{
    public Task<decimal> GetCurrencyConversionRate(string currencyFrom, string currencyTo, CancellationToken ct = default);
    public Task<IEnumerable<CurrencyRateEntity>> GetCurrentCurrencyRates(CancellationToken ct = default);
    public Task UpdateCurrencyRates(CancellationToken ct = default);
}
