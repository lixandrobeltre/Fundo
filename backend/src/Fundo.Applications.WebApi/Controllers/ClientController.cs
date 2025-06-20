using FluentValidation;
using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                var client = await clientService.CreateClientAsync(dto);
                return CreatedAtAction(nameof(GetClient), new { code = client.Code }, client);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetClient([FromRoute] string code)
        {
            try
            {
                var client = await clientService.GetClient(code);
                return Ok(client);

            }
            catch (NotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
