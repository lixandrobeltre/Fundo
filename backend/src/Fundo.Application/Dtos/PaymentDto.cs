#nullable disable

namespace Fundo.Application.Dtos;

public class PaymentDto
{
    public LoanDto Loan { get; set; }

    public decimal PaymentAmount { get; set; }

    public string Description { get; set; }

    public decimal PrincipalPayment { get; set; }

    public decimal InterestPayment { get; set; }

    public Guid RowId { get; set; }
}