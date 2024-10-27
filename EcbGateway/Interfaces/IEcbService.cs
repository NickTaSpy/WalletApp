using EcbGateway.Models;
using System.Threading.Tasks;
using System.Threading;

namespace EcbGateway.Interfaces;

public interface IEcbService
{
    public Task<CurrenciesRates> GetCurrenciesRates(CancellationToken ct = default);
}
