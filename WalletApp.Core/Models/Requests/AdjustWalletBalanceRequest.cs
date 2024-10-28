namespace WalletApp.Core.Models.Requests;

public class AdjustWalletBalanceRequest
{
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
