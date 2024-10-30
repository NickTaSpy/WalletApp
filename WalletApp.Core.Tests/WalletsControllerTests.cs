using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net.Http.Json;
using WalletApp.Core.Models;
using WalletApp.Core.Models.Requests;
using WalletApp.Infrastructure.Database;

namespace WalletApp.Web.Tests;

public class WalletsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WalletsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Change culture for balance parsing to work properly.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Fact]
    public async Task CreateWallet_Returns_Valid_Wallet()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        var request = new CreateWalletRequest
        {
            Balance = 1000m,
            Currency = "EUR"
        };

        // Act
        var result = await _client.PostAsJsonAsync("api/wallets", request);

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var content = await result.Content.ReadFromJsonAsync<Wallet>();
        content.Should().BeEquivalentTo(new Wallet
        {
            Balance = request.Balance,
            Currency = request.Currency
        }, options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task RetrieveWalletBalance_Returns_Valid_Balance()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.GetAsync($"api/wallets/{2}");

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var resultBalance = await result.Content.ReadAsStringAsync();
        decimal.Parse(resultBalance).Should().Be(1500m);
    }

    [Fact]
    public async Task RetrieveWalletBalance_Returns_Valid_Balance_With_Currency_Exchange()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.GetAsync($"api/wallets/{2}?currency=EUR");

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var resultBalance = await result.Content.ReadAsStringAsync();
        decimal.Parse(resultBalance).Should().Be(1500m / 2m);
    }

    [Fact]
    public async Task AdjustWalletBalance_Returns_Valid_Balance_With_AddFundsStrategy()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.PostAsync($"api/wallets/{2}/adjustBalance?amount={500}&currency=EUR&strategy={nameof(AdjustFundsStrategy.AddFundsStrategy)}", null);

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var resultBalance = await result.Content.ReadAsStringAsync();
        decimal.Parse(resultBalance).Should().Be(1500m + (500m * 2m));
    }

    [Fact]
    public async Task AdjustWalletBalance_Returns_Valid_Balance_With_SubtractFundsStrategy()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.PostAsync($"api/wallets/{2}/adjustBalance?amount={500}&currency=EUR&strategy={nameof(AdjustFundsStrategy.SubtractFundsStrategy)}", null);

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var resultBalance = await result.Content.ReadAsStringAsync();
        decimal.Parse(resultBalance).Should().Be(1500m - (500m * 2m));
    }

    [Fact]
    public async Task AdjustWalletBalance_Fails_When_Negative_Balance_With_SubtractFundsStrategy()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.PostAsync($"api/wallets/{2}/adjustBalance?amount={2000}&currency=EUR&strategy={nameof(AdjustFundsStrategy.SubtractFundsStrategy)}", null);

        // Assert
        result.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task AdjustWalletBalance_Returns_Valid_Balance_With_ForceSubtractFundsStrategy()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<WalletAppDbContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        // Act
        var result = await _client.PostAsync($"api/wallets/{2}/adjustBalance?amount={2000}&currency=EUR&strategy={nameof(AdjustFundsStrategy.ForceSubtractFundsStrategy)}", null);

        // Assert
        result.IsSuccessStatusCode.Should().BeTrue();
        var resultBalance = await result.Content.ReadAsStringAsync();
        decimal.Parse(resultBalance).Should().Be(1500m - (2000m * 2m));
    }
}