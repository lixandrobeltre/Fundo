using Fundo.Application.Exceptions;
using Fundo.Application.Services;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Services
{
    public class UserServiceTests
    {
        private readonly FundoLoanDbContext dbContext;
        private readonly UserService userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<FundoLoanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new FundoLoanDbContext(options);

            var password = "TestPassword123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            dbContext.Users.Add(new User
            {
                Code = "U001",
                Name = "Test User",
                Username = "testuser",
                PasswordHash = passwordHash,
                Status = true,
                RowId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });

            dbContext.SaveChanges();

            userService = new UserService(dbContext);
        }

        [Fact]
        public async Task ValidateUserAsync_ReturnsUserDto_WhenCredentialsAreValid()
        {
            // Act
            var result = await userService.ValidateUserAsync("testuser", "TestPassword123!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("U001", result.Code);
            Assert.Equal("Test User", result.Name);
            Assert.Equal("testuser", result.Username);
            Assert.True(result.Status);
        }

        [Fact]
        public async Task ValidateUserAsync_ThrowsNotFoundException_WhenUserDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                userService.ValidateUserAsync("nonexistent", "irrelevant"));
        }

        [Fact]
        public async Task ValidateUserAsync_ThrowsInvalidDataException_WhenPasswordIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() =>
                userService.ValidateUserAsync("testuser", "WrongPassword"));
        }
    }
}
