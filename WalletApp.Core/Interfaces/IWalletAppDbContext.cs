using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using WalletApp.Core.Entities;

namespace WalletApp.Core.Interfaces;

public interface IWalletAppDbContext
{
    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }

    public Task<int> SaveAsync(CancellationToken ct = default);
}
