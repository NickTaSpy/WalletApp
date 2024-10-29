namespace WalletApp.Core.Models.Requests;

public class CreateWalletRequest
{
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "EUR";
}
