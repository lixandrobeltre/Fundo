namespace Fundo.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(string loanCode, decimal amount, string description);
    }
}
