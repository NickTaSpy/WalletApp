namespace WalletApp.Core.Models.Requests;

public class AdjustWalletBalanceRequest
{
    public long WalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public AdjustFundsStrategy? Strategy { get; set; }
}

public enum AdjustFundsStrategy
{
    AddFundsStrategy = 0,
    SubtractFundsStrategy = 1,
    ForceSubtractFundsStrategy = 2,
}
