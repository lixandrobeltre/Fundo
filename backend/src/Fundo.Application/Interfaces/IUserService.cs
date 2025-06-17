using Fundo.Application.Dtos;

namespace Fundo.Application.Interfaces
{
    public interface IUserService
    {   
        Task<UserDto> ValidateUserAsync(string username, string password);
    }
}
