using Fundo.Application.Interfaces;
using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<AuthController> logger;
        private readonly IConfiguration configuration;
        private readonly JwtOption jwtOption;

        public AuthController(IUserService userService, ILogger<AuthController> logger, IConfiguration configuration, IOptions<JwtOption> jwtOption)
        {
            this.userService = userService;
            this.logger = logger;
            this.configuration = configuration;
            this.jwtOption = jwtOption.Value;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromForm] string username, [FromForm] string password)
        {
            try
            {
                var user = await userService.ValidateUserAsync(username, password);

                var claims = new[]
                {
                    new Claim(ClaimTypes.GivenName, user.Username),
                    new Claim(JwtRegisteredClaimNames.Actort, user.Username),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("SecurityKey").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtOption.Issuer,
                    audience: jwtOption.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(jwtOption.ExpiresInMinutes),
                    signingCredentials: creds);

                return Ok(new
                {
                    access_token = new JwtSecurityTokenHandler().WriteToken(token),
                    token_type = "Bearer",
                    expires_in = 1800
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Authentication failed for user {Username}", username);
                return Unauthorized();
            }
        }
    }
}
