using Fundo.Application.Dtos;
using Fundo.Application.Interfaces;
using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/loans")]
    public class LoanManagementController : ControllerBase
    {
        private readonly ILoanService loanService;
        private readonly IPaymentService paymentService;

        public LoanManagementController(ILoanService loanService, IPaymentService paymentService)
        {
            this.loanService = loanService;
            this.paymentService = paymentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanByCode([FromRoute] string id)
        {
            var loan = await loanService.GetLoanDetailsAsync(id);
            return Ok(loan);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto dto)
        {
            var code = await loanService.CreateLoanAsync(dto);
            return CreatedAtAction(nameof(GetLoanByCode), new { id = code }, new { id = code });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLoans([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var loans = await loanService.GetAllLoansAsync(page, pageSize);
            return Ok(loans);
        }

        [HttpPost("{id}/payment")]
        public async Task<IActionResult> MakePayment([FromRoute] string id, [FromBody] PaymentRequest request)
        {
            var result = await paymentService.ProcessPaymentAsync(id, request.Amount, request.Description);
            return result ? Ok() : BadRequest();
        }
    }
}