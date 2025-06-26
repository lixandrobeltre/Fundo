using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Domain.Enums;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly PaymentService paymentService;
        private readonly Mock<ILogin> loginMock;

        public PaymentServiceTests()
        {
            var options = new DbContextOptionsBuilder<FundoLoanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new FundoLoanDbContext(options);

            var client = new Client
            {
                Code = "C001",
                Identification = "123456789",
                Name = "Test Client",
                Email = "test@client.com",
                RowId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            };
            dbContext.Clients.Add(client);

            var loan = new Loan
            {
                IdClient = client.Id,
                Code = "L001",
                LoanType = "Personal",
                Status = LoanStatus.Active.ToString(),
                OriginalAmount = 1000m,
                OutstandingBalance = 1000m,
                InterestRate = 12m,
                Term = 12,
                PaymentAmount = 100m,
                PaymentDay = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed",
                RowId = Guid.NewGuid()
            };
            dbContext.Loans.Add(loan);
            dbContext.SaveChanges();

            loginMock = new Mock<ILogin>();
            loginMock.Setup(l => l.GetUsername()).Returns("testuser");

            paymentService = new PaymentService(dbContext, loginMock.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_SuccessfulPayment_UpdatesLoanAndCreatesPayment()
        {
            // Act
            var result = await paymentService.ProcessPaymentAsync("L001", 200m, "First payment");

            // Assert
            Assert.True(result);

            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Code == "L001");
            Assert.NotNull(loan);
            Assert.True(loan.OutstandingBalance < 1000m);
            Assert.Equal("Active", loan.Status);

            var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.IdLoan == loan.Id);

            Assert.NotNull(payment);
            Assert.Equal(200m, payment.PaymentAmount);
            Assert.Equal("First payment", payment.Description);
        }

        [Fact]
        public async Task ProcessPaymentAsync_FullyPaysLoan_UpdatesStatusToPaid()
        {
            // Arrange
            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Code == "L001");
            loan.OutstandingBalance = 100m;
            dbContext.SaveChanges();

            // Act
            var result = await paymentService.ProcessPaymentAsync("L001", 200m, "Final payment");

            // Assert
            Assert.True(result);
            loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Code == "L001");
            Assert.Equal(0m, loan.OutstandingBalance);
            Assert.Equal("Paid", loan.Status);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ThrowsValidationException_WhenLoanNotFound()
        {
            await Assert.ThrowsAsync<ValidationException>(() =>
                paymentService.ProcessPaymentAsync("NON_EXISTENT", 100m, "Invalid loan"));
        }

        [Fact]
        public async Task ProcessPaymentAsync_ThrowsInvalidOperationException_WhenLoanAlreadyPaid()
        {
            // Arrange
            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Code == "L001");
            loan.OutstandingBalance = 0m;
            loan.Status = "Paid";
            dbContext.SaveChanges();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                paymentService.ProcessPaymentAsync("L001", 100m, "Should fail"));
        }
    }
}
