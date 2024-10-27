using Microsoft.EntityFrameworkCore;
using WalletApp.Infrastructure.Database.Tables;

namespace WalletApp.Infrastructure.Database;

public class WalletAppDbContext : DbContext
{
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<CurrencyRate> CurrencyRates { get; set; }

    public WalletAppDbContext(DbContextOptions<WalletAppDbContext> options) : base(options)
    {
    }
}
