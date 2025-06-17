using Fundo.Application.Interfaces;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Fundo.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly FundoLoanDbContext dbContext;

        public PaymentService(FundoLoanDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ProcessPaymentAsync(string loanCode, decimal amount, string description)
        {
            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Code == loanCode)
                ?? throw new ValidationException("Loan not found");

            if (loan.OutstandingBalance <= 0)
                throw new InvalidOperationException("Loan is already fully paid");

            decimal interestPayment = loan.OutstandingBalance * (loan.InterestRate / 100) / 12;
            decimal principalPayment = amount > interestPayment ? amount - interestPayment : 0;

            var payment = new Payment
            {
                IdLoan = loan.Id,
                PaymentAmount = amount,
                Description = description,
                PrincipalPayment = principalPayment,
                InterestPayment = interestPayment,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system", 
                RowId = Guid.NewGuid()
            };

            loan.OutstandingBalance -= principalPayment;
            loan.LastPaymentDate = DateTime.UtcNow;
            loan.ModifiedAt = DateTime.UtcNow;
            loan.ModifiedBy = "system";

            if (loan.OutstandingBalance <= 0)
            {
                loan.OutstandingBalance = 0;
                loan.Status = "Paid";
            }

            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
