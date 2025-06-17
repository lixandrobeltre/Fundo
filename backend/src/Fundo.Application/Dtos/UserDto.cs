#nullable disable

namespace Fundo.Application.Dtos;

public class UserDto
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Username { get; set; }

    public bool Status { get; set; }

    public Guid RowId { get; set; }
}