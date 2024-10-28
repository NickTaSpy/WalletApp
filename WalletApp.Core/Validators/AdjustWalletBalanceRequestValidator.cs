using FluentValidation;
using WalletApp.Core.Models.Requests;

namespace WalletApp.Core.Validators;

public class AdjustWalletBalanceRequestValidator : AbstractValidator<AdjustWalletBalanceRequest>
{
    public AdjustWalletBalanceRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Strategy).NotNull();
        RuleFor(x => x.Currency).Length(3).When(x => x.Currency is not null);
    }
}
