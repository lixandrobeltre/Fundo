using Fundo.Application.Dtos;

namespace Fundo.Application.Interfaces
{
    public interface ILoanService
    {
        Task<LoanDto> GetLoanDetailsAsync(string code);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync(int page, int pageSize);
        Task<string> CreateLoanAsync(CreateLoanDto loanDto);
    }
}
