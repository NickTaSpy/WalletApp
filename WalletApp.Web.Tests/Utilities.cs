using WalletApp.Core.Entities;
using WalletApp.Infrastructure.Database;

namespace WalletApp.Web.Tests;

public static class Utilities
{
    public static void InitializeDbForTests(WalletAppDbContext db)
    {
        db.Wallets.AddRange(GetSeedingWallets());
        db.CurrencyRates.AddRange(GetSeedingCurrencies());
        db.SaveChanges();
    }

    public static void ReinitializeDbForTests(WalletAppDbContext db)
    {
        db.Wallets.RemoveRange(db.Wallets);
        db.CurrencyRates.RemoveRange(db.CurrencyRates);
        InitializeDbForTests(db);
    }

    public static List<WalletEntity> GetSeedingWallets()
    {
        return
        [
            new()
            {
                Id = 1,
                Balance = 1000m,
                Currency = "EUR"
            },
            new()
            {
                Id = 2,
                Balance = 1500m,
                Currency = "USD"
            },
        ];
    }

    public static List<CurrencyRateEntity> GetSeedingCurrencies()
    {
        return
        [
            new()
            {
                Id = 1,
                Currency = "USD",
                Date = DateTime.Now,
                Rate = 2m
            },
        ];
    }
}
