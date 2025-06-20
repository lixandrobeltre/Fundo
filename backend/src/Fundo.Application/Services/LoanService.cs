using FluentValidation;
using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Application.Validators;
using Fundo.Domain.Enums;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fundo.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly LoanValidator validations;
        private readonly ILogger<LoanService> logger;

        public LoanService(FundoLoanDbContext dbContext, LoanValidator validations, ILogger<LoanService> logger)
        {
            this.dbContext = dbContext;
            this.validations = validations;
            this.logger = logger;
        }

        public async Task<Guid> CreateLoanAsync(CreateLoanDto loanDto)
        {
            try
            {
                var result = validations.Validate(loanDto);

                if (!result.IsValid)
                    throw new ValidationException(string.Join(",", result.Errors.Select(s => s.ErrorMessage)));

                var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Code == loanDto.ClientCode)
                    ?? throw new ValidationException("Client not found");

                var loan = new Loan
                {
                    Code = $"LN-{Guid.NewGuid().ToString()[..8]}",
                    LoanType = loanDto.LoanType,
                    Status = LoanStatus.Active.ToString(),
                    OriginalAmount = loanDto.OriginalAmount,
                    OutstandingBalance = loanDto.OriginalAmount,
                    InterestRate = loanDto.InterestRate,
                    PaymentAmount = CalculatePaymentAmount(loanDto.OriginalAmount, loanDto.InterestRate, loanDto.Term),
                    PaymentDay = loanDto.PaymentDay,
                    Term = loanDto.Term,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system", // TODO: Replace with actual user context
                    RowId = Guid.NewGuid()
                };

                dbContext.Loans.Add(loan);
                await dbContext.SaveChangesAsync();
                return loan.RowId;
            }
            catch (Exception e) when (e is not ValidationException)
            {
                logger.LogError(e, "{message}", e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync(int page, int pageSize)
        {
            try
            {
                var loans = await dbContext.Loans
                        .Include(l => l.IdClientNavigation)
                        .OrderByDescending(l => l.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(loan => new LoanDto
                        {
                            Client = new ClientDto
                            {
                                Code = loan.IdClientNavigation.Code,
                                Identification = loan.IdClientNavigation.Identification,
                                Name = loan.IdClientNavigation.Name,
                                Email = loan.IdClientNavigation.Email,
                                RowId = loan.IdClientNavigation.RowId
                            },
                            Code = loan.Code,
                            LoanType = loan.LoanType,
                            Status = loan.Status,
                            OriginalAmount = loan.OriginalAmount,
                            OutstandingBalance = loan.OutstandingBalance,
                            InterestRate = loan.InterestRate,
                            PaymentAmount = loan.PaymentAmount,
                            PaymentDay = loan.PaymentDay,
                            PaymentDueDate = loan.PaymentDueDate,
                            LastPaymentDate = loan.LastPaymentDate,
                            RowId = loan.RowId
                        })
                        .ToListAsync();

                return loans;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{message}", e.Message);
                throw;
            }
        }

        public async Task<LoanDto> GetLoanDetailsAsync(string code)
        {
            try
            {
                var loan = await dbContext.Loans
                        .Include(l => l.IdClientNavigation)
                        .FirstOrDefaultAsync(l => l.Code == code)
                        ?? throw new ValidationException("Loan not found");

                return new LoanDto
                {
                    Client = new ClientDto
                    {
                        Code = loan.IdClientNavigation.Code,
                        Identification = loan.IdClientNavigation.Identification,
                        Name = loan.IdClientNavigation.Name,
                        Email = loan.IdClientNavigation.Email,
                        RowId = loan.IdClientNavigation.RowId
                    },
                    Code = loan.Code,
                    LoanType = loan.LoanType,
                    Status = loan.Status,
                    OriginalAmount = loan.OriginalAmount,
                    OutstandingBalance = loan.OutstandingBalance,
                    InterestRate = loan.InterestRate,
                    PaymentAmount = loan.PaymentAmount,
                    PaymentDay = loan.PaymentDay,
                    PaymentDueDate = loan.PaymentDueDate,
                    LastPaymentDate = loan.LastPaymentDate,
                    RowId = loan.RowId
                };
            }
            catch (Exception e) when (e is not ValidationException)
            {
                logger.LogError(e, "{message}", e.Message);
                throw;
            }
        }

        static decimal CalculatePaymentAmount(decimal originalAmount, decimal interestRate, short term)
        {
            // Simple interest calculation for demonstration purposes
            var totalInterest = originalAmount * (interestRate / 100) * (term / 12.0m);
            var totalAmount = originalAmount + totalInterest;
            return totalAmount / term;
        }
    }
}
