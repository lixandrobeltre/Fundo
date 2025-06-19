#nullable disable

namespace Fundo.Application.Dtos;

public class CreateClientDto
{
    public string Code { get; set; }

    public string Identification { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
}