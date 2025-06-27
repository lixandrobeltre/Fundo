using Fundo.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fundo.Applications.WebApi.Services
{
    public class LoginService : ILogin
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public LoginService(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public string GetUsername()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value
                ?? httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Actort)?.Value
                ?? httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
    }
}
