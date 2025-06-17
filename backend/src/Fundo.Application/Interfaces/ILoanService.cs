using Fundo.Application.Dtos;

namespace Fundo.Application.Interfaces
{
    internal interface ILoanService
    {
        Task<LoanDto> GetLoanDetailsAsync(string code);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync(int page, int pageSize);
        Task<Guid> CreateLoanAsync(CreateLoanDto loanDto);
    }
}
