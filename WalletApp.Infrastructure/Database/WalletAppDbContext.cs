using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Entities;
using WalletApp.Core.Interfaces;

namespace WalletApp.Infrastructure.Database;

public class WalletAppDbContext : DbContext, IWalletAppDbContext
{
    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }

    public WalletAppDbContext(DbContextOptions<WalletAppDbContext> options) : base(options)
    {
    }

    public async Task<int> SaveAsync(CancellationToken ct = default)
    {
        return await SaveChangesAsync(ct);
    }
}
