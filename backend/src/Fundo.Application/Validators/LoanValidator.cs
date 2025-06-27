using FluentValidation;
using Fundo.Application.Dtos;

namespace Fundo.Application.Validators
{
    public class LoanValidator : AbstractValidator<CreateLoanDto>
    {
        public LoanValidator()
        {
            RuleFor(x => x.OriginalAmount).GreaterThan(0);
            RuleFor(x => x.InterestRate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.LoanType).NotEmpty();
            RuleFor(x => x.ClientCode).NotEmpty();
            RuleFor(x => x.Term).GreaterThan((short)0);
            RuleFor(x => x.PaymentDay).InclusiveBetween((short)1, (short)30);
        }
    }
}
