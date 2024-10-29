using EcbGateway.Models;
using System.Threading.Tasks;
using System.Threading;

namespace WalletApp.Core.Interfaces;

public interface ICurrencyDataService
{
    public Task SaveOrUpdateCurrenciesRates(CurrenciesRates currenciesRates, CancellationToken ct = default);
}
