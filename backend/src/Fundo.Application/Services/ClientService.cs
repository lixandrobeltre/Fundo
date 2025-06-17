using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly FundoLoanDbContext dbContext;

        public ClientService(FundoLoanDbContext dbContext) => this.dbContext = dbContext;

        public async Task<ClientDto> GetClient(string code)
        {
            var client = await dbContext.Clients
                .Where(c => c.Code == code)
                .Select(c => new ClientDto
                {
                    Code = c.Code,
                    Identification = c.Identification,
                    Name = c.Name,
                    Email = c.Email,
                    RowId = c.RowId
                })
                .FirstOrDefaultAsync() 
                ?? throw new NotFoundException("User no found");

                return client;
        }
    }
}
