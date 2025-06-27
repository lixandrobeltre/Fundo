namespace Fundo.Applications.WebApi.Models;

public class JwtOption
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 30;
}
