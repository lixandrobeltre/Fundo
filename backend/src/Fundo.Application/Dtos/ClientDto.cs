#nullable disable

namespace Fundo.Application.Dtos;

public class ClientDto
{
    public string Code { get; set; }

    public string Identification { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public Guid RowId { get; set; }
}