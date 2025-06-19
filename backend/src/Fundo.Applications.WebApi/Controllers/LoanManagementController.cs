using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fundo.Application.Interfaces;

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
        public async Task<IActionResult> GetLoanById([FromRoute] string id)
        {
            var loan = await loanService.GetLoanDetailsAsync(id);
            return Ok(loan);
        }
    }
}