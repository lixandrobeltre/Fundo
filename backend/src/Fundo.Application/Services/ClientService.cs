using FluentValidation;
using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fundo.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly ILogger<ClientService> logger;
        private readonly ILogin login;

        public ClientService(FundoLoanDbContext dbContext, ILogger<ClientService> logger, ILogin login)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.login = login;
        }

        public async Task<ClientDto> GetClient(string code)
        {
            try
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
            catch (Exception e) when (e is not NotFoundException)
            {
                logger.LogError(e, "{message}", e.Message);
                throw;
            }
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientDto dto)
        {
            try
            {
                if (await dbContext.Clients.AnyAsync(c => c.Code == dto.Code))
                    throw new ValidationException("A client with this code already exists.");

                var client = new Client
                {
                    Code = dto.Code,
                    Identification = dto.Identification,
                    Name = dto.Name,
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = login.GetUsername(),
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
            catch (Exception e) when (e is not ValidationException)
            {
                logger.LogError(e, "{message}", e.Message);
                throw;
            }
        }
    }
}
