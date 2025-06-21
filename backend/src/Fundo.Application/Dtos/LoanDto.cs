#nullable disable

namespace Fundo.Application.Dtos;

public class LoanDto
{
    public ClientDto Client { get; set; }

    public string Code { get; set; }

    public string LoanType { get; set; }

    public string Status { get; set; }

    public decimal OriginalAmount { get; set; }

    public decimal OutstandingBalance { get; set; }

    public decimal InterestRate { get; set; }

    public decimal PaymentAmount { get; set; }

    public short PaymentDay { get; set; }

    public DateTime? PaymentDueDate { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    public Guid RowId { get; set; }
}