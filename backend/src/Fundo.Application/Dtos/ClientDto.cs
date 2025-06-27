#nullable disable

namespace Fundo.Application.Dtos;

public class ClientDto : CreateClientDto
{
    public Guid RowId { get; set; }
}