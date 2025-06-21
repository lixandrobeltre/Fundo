using Bogus;
using Fundo.Application.Dtos;

namespace Fundo.Services.Tests.Common
{
    internal static class FakerData
    {
        public static CreateLoanDto GenerateLoanDto(string clientCode)
        {
            var loanFaker = new Faker<CreateLoanDto>()
                .RuleFor(p => p.ClientCode, clientCode)
                .RuleFor(p => p.LoanType, f => f.PickRandom("Personal", "Auto", "Mortgage"))
                .RuleFor(p => p.OriginalAmount, f => f.Finance.Amount(1000, 100000))
                .RuleFor(p => p.InterestRate, f => f.Random.Decimal(1, 10))
                .RuleFor(p => p.PaymentDay, f => f.Random.Short(1, 28))
                .RuleFor(p => p.Term, f => f.Random.Short(6, 360));

            var loanDto = loanFaker.Generate();

            return loanDto;
        }

        public static CreateClientDto GenerateClientDto() {
            var clientFaker = new Faker<CreateClientDto>()
                .RuleFor(c => c.Code, f => f.Random.AlphaNumeric(8))
                .RuleFor(c => c.Identification, f => f.Random.AlphaNumeric(12))
                .RuleFor(c => c.Name, f => f.Person.FullName)
                .RuleFor(c => c.Email, f => f.Internet.Email());

            var clientDto = clientFaker.Generate();
            return clientDto;
        }
    }
}
