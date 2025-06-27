#nullable disable

namespace Fundo.Application.Dtos;

public class CreateLoanDto
{
    public string ClientCode { get; set; }

    public string LoanType { get; set; }

    public decimal OriginalAmount { get; set; }

    public decimal InterestRate { get; set; }
    public short PaymentDay { get; set; }
    public short Term { get; set; }
}