namespace WalletApp.Core.Models;

public class Wallet
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "EUR";
}
