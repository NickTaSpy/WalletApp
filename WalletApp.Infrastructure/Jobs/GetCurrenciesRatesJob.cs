using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using WalletApp.Core.Interfaces;

namespace WalletApp.Infrastructure.Jobs;

public class GetCurrenciesRatesJob : IJob
{
    public static readonly JobKey Key = new(nameof(GetCurrenciesRatesJob));

    private readonly ILogger<GetCurrenciesRatesJob> _logger;
    private readonly ICurrencyService _currencyService;

    public GetCurrenciesRatesJob(
        ILogger<GetCurrenciesRatesJob> logger,
        ICurrencyService currencyService)
    {
        _logger = logger;
        _currencyService = currencyService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Job executed.");
        await _currencyService.UpdateCurrencyRates();
    }
}
