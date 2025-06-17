namespace Fundo.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(Guid loandId, decimal amount, string description);
    }
}
