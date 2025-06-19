using Fundo.Application.Dtos;
using Fundo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientController(IClientService clientService) => this.clientService = clientService;

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDto dto)
        {
            var client = await clientService.CreateClientAsync(dto);
            return CreatedAtAction(nameof(GetClient), new { code = client.Code }, client);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetClient([FromRoute] string code)
        {
            var client = await clientService.GetClient(code);
            return Ok(client);
        }
    }
}
