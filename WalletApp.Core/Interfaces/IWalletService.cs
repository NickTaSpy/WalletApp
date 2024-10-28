﻿using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Models;
using WalletApp.Core.Models.Requests;

namespace WalletApp.Core.Interfaces;

public interface IWalletService
{
    public Task<Wallet> CreateWallet(CancellationToken ct = default);
    public Task<decimal> RetrieveWalletBalance(long walletId, string? currency, CancellationToken ct = default);
    public Task<decimal> AdjustWalletBalance(AdjustWalletBalanceRequest request, CancellationToken ct = default);
}
