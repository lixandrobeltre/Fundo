using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly ClientService clientService;
        private readonly Mock<ILogger<ClientService>> loggerMock;
        private readonly Mock<ILogin> loginMock;

        public ClientServiceTests()
        {
            var options = new DbContextOptionsBuilder<FundoLoanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            dbContext = new FundoLoanDbContext(options);

            // Seed data
            dbContext.Clients.Add(new Client
            {
                Code = "C001",
                Identification = "123456789",
                Name = "Test Client",
                Email = "test@client.com",
                CreatedBy = "system",
                RowId = Guid.NewGuid()
            });
            dbContext.SaveChanges();

            loggerMock = new Mock<ILogger<ClientService>>();
            loginMock = new Mock<ILogin>();
            clientService = new ClientService(dbContext, loggerMock.Object, loginMock.Object);
        }

        [Fact]
        public async Task GetClient_ReturnsClientDto_WhenClientExists()
        {
            // Act
            var result = await clientService.GetClient("C001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C001", result.Code);
            Assert.Equal("Test Client", result.Name);
        }

        [Fact]
        public async Task GetClient_ThrowsNotFoundException_WhenClientDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => clientService.GetClient("NON_EXISTENT"));
        }
    }
}
