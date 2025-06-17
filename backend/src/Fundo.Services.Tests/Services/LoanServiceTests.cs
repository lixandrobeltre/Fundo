using Fundo.Application.Dtos;
using Fundo.Application.Services;
using Fundo.Application.Validators;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Services
{
    public class LoanServiceTests
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly LoanService loanService;
        private readonly Mock<LoanValidator> validatorMock;

        public LoanServiceTests()
        {
            var options = new DbContextOptionsBuilder<FundoLoanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new FundoLoanDbContext(options);

            // Seed a client
            dbContext.Clients.Add(new Client
            {
                Id = 1,
                Code = "C001",
                Identification = "123456789",
                Name = "Test Client",
                Email = "test@client.com",
                RowId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });
            dbContext.SaveChanges();

            // Mock LoanValidator
            validatorMock = new Mock<LoanValidator>();
            validatorMock.Setup(v => v.Validate(It.IsAny<CreateLoanDto>()))
                .Returns(new ValidationResult(string.Empty));

            loanService = new LoanService(dbContext, validatorMock.Object);
        }

        [Fact]
        public async Task CreateLoanAsync_CreatesLoan_WhenValid()
        {
            var dto = new CreateLoanDto
            {
                ClientCode = "C001",
                LoanType = "Personal",
                OriginalAmount = 1000m,
                InterestRate = 10m,
                PaymentDay = 1,
                Term = 12
            };

            var loanId = await loanService.CreateLoanAsync(dto);

            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.RowId == loanId);
            Assert.NotNull(loan);
            Assert.Equal("Personal", loan.LoanType);
            Assert.Equal(1000m, loan.OriginalAmount);
        }

        [Fact]
        public async Task CreateLoanAsync_ThrowsValidationException_WhenInvalid()
        {
            // Arrange
            validatorMock.Setup(v => v.Validate(It.IsAny<CreateLoanDto>()))
                .Returns(new ValidationResult(new List<ValidationFailure> {
                    new ValidationFailure("LoanType", "LoanType is required")
                }));

            var dto = new CreateLoanDto
            {
                ClientCode = "C001",
                LoanType = null,
                OriginalAmount = 1000m,
                InterestRate = 10m,
                PaymentDay = 1,
                Term = 12
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => loanService.CreateLoanAsync(dto));
        }

        [Fact]
        public async Task CreateLoanAsync_ThrowsValidationException_WhenClientNotFound()
        {
            var dto = new CreateLoanDto
            {
                ClientCode = "NON_EXISTENT",
                LoanType = "Personal",
                OriginalAmount = 1000m,
                InterestRate = 10m,
                PaymentDay = 1,
                Term = 12
            };

            await Assert.ThrowsAsync<ValidationException>(() => loanService.CreateLoanAsync(dto));
        }

        [Fact]
        public async Task GetAllLoansAsync_ReturnsPagedLoans()
        {
            dbContext.Loans.Add(new Loan
            {
                IdClient = 1,
                Code = "L001",
                LoanType = "Personal",
                Status = "Active",
                OriginalAmount = 1000m,
                OutstandingBalance = 1000m,
                InterestRate = 10m,
                Term = 12,
                PaymentAmount = 100m,
                PaymentDay = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed",
                RowId = Guid.NewGuid()
            });
            dbContext.SaveChanges();

            var result = await loanService.GetAllLoansAsync(1, 10);
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetLoanDetailsAsync_ReturnsLoanDto_WhenExists()
        {
            var loan = new Loan
            {
                Id = 2,
                IdClient = 1,
                Code = "L002",
                LoanType = "Auto",
                Status = "Active",
                OriginalAmount = 5000m,
                OutstandingBalance = 5000m,
                InterestRate = 8m,
                Term = 24,
                PaymentAmount = 220m,
                PaymentDay = 5,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed",
                RowId = Guid.NewGuid()
            };
            dbContext.Loans.Add(loan);
            dbContext.SaveChanges();

            var result = await loanService.GetLoanDetailsAsync("L002");
            Assert.NotNull(result);
            Assert.Equal("L002", result.Code);
            Assert.Equal("Auto", result.LoanType);
        }

        [Fact]
        public async Task GetLoanDetailsAsync_ThrowsValidationException_WhenNotFound()
        {
            await Assert.ThrowsAsync<ValidationException>(() => loanService.GetLoanDetailsAsync("NON_EXISTENT"));
        }
    }
