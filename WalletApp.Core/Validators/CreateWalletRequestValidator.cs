using FluentValidation;
using WalletApp.Core.Models.Requests;

namespace WalletApp.Core.Validators;

public class CreateWalletRequestValidator : AbstractValidator<CreateWalletRequest>
{
    public CreateWalletRequestValidator()
    {
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).Length(3);
    }
}
