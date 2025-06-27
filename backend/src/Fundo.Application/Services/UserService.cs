using Fundo.Application.Dtos;
using Fundo.Application.Exceptions;
using Fundo.Application.Interfaces;
using Fundo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly FundoLoanDbContext dbContext;

        public UserService(FundoLoanDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserDto> ValidateUserAsync(string username, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username)
                ?? throw new NotFoundException("User not found");

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new InvalidDataException();

            var model = new UserDto
            {
                Code = user.Code,
                Name = user.Name,
                Username = user.Username,
                Status = user.Status,
                RowId = user.RowId
            };

            return model;
        }
    }
}
