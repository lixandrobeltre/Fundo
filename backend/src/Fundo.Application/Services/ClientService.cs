using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Domain.Models;
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

        public async Task<ClientDto> CreateClientAsync(CreateClientDto dto)
        {
            if (await dbContext.Clients.AnyAsync(c => c.Code == dto.Code))
                throw new ApplicationException("A client with this code already exists.");

            var client = new Client
            {
                Code = dto.Code,
                Identification = dto.Identification,
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system",
                RowId = Guid.NewGuid()
            };

            dbContext.Clients.Add(client);
            await dbContext.SaveChangesAsync();

            return new ClientDto
            {
                Code = client.Code,
                Identification = client.Identification,
                Name = client.Name,
                Email = client.Email,
                RowId = client.RowId
            };
        }
    }
}
