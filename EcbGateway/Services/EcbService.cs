using EcbGateway.Interfaces;
using EcbGateway.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EcbGateway.Services;

public class EcbService : IEcbService
{
    private readonly ILogger<EcbService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EcbGatewaySettings _settings;

    public EcbService(
        ILogger<EcbService> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<EcbGatewaySettings> settings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<CurrenciesRates> GetCurrenciesRates(CancellationToken ct = default)
    {
        _logger.LogInformation("Getting currency rates from URL: {url}", _settings.EcbRatesUrl);

        var http = _httpClientFactory.CreateClient(Constants.EcbHttpClientName);

        await using var stream = await http.GetStreamAsync(_settings.EcbRatesUrl, ct);

        var serializer = new XmlSerializer(typeof(EcbRatesResponse));

        if (serializer.Deserialize(stream) is not EcbRatesResponse ecbResponse)
        {
            throw new EcbGatewayException("Failed to deserialize ECB response data.");
        }

        var result = ParseEcbResponse(ecbResponse);

        _logger.LogInformation("Retrieved {count} currency rates for date {date}",
            result.CurrencyRates.Count, result.Date);

        return result;
    }

    private CurrenciesRates ParseEcbResponse(EcbRatesResponse ecbResponse)
    {
        if (ecbResponse.Cube?.CubeDate is null ||
            ecbResponse.Cube.CubeDate.Date == default ||
            ecbResponse.Cube.CubeDate.Rates is null ||
            ecbResponse.Cube.CubeDate.Rates.Length == 0)
        {
            throw new EcbGatewayException("Failed to deserialize ECB response data.");
        }

        var result = new CurrenciesRates();
        result.Date = ecbResponse.Cube.CubeDate.Date;

        foreach (var rate in ecbResponse.Cube.CubeDate.Rates)
        {
            if (string.IsNullOrEmpty(rate.Currency))
            {
                _logger.LogWarning("Failed to parse currency rate. | Currency: {currency} | Rate: {rate}",
                    rate.Currency, rate.Rate);
                continue;
            }

            result.CurrencyRates.Add(rate.Currency, rate.Rate);
        }

        return result;
    }
}
