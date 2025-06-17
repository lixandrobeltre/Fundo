using Fundo.Application.Dtos;

namespace Fundo.Application.Interfaces
{
    public interface IClientService
    {
        Task<ClientDto> GetClient(string code);
    }
}
