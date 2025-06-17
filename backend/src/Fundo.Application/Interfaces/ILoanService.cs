using Fundo.Application.Dtos;

namespace Fundo.Application.Interfaces
{
    internal interface ILoanService
    {
        Task<LoanDto> GetLoanDetailsAsync(Guid loanId);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<bool> CreateLoanAsync(LoanDto loanDto);
    }
}
