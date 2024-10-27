using EcbGateway.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EcbGateway.Tests;

public class EcbServiceTests
{
    private readonly IEcbService _sut;

    public EcbServiceTests()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddEcbGateway();
        var provider = builder.Services.BuildServiceProvider();
        _sut = provider.CreateScope().ServiceProvider.GetRequiredService<IEcbService>();
    }

    [Fact]
    public async Task GetCurrenciesRates_Should_Return_Valid_Results()
    {
        // Arrange

        // Act
        var result = await _sut.GetCurrenciesRates();

        // Assert
        result.Date.Should().NotBeSameDateAs(default);
        result.CurrencyRates.Should().OnlyContain(pair => pair.Value > 0m);
        result.CurrencyRates.Should().ContainKeys(
            "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "PLN", "RON", "SEK",
            "CHF", "ISK", "NOK", "TRY", "AUD", "BRL", "CAD", "CNY", "HKD", "IDR",
            "ILS", "INR", "KRW", "MXN", "MYR", "NZD", "PHP", "SGD", "THB", "ZAR");
    }
}