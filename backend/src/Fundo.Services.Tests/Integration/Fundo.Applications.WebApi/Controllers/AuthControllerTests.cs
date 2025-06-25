using Fundo.Applications.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public AuthControllerTests(WebApplicationFactory<Startup> factory)
    {
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Authenticate_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("username", "validuser"),
            new KeyValuePair<string, string>("password", "validpass")
            ]);

        // Act
        var response = await client.PostAsync("/auth/Authenticate", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("access_token", responseBody);
        Assert.Contains("Bearer", responseBody);
    }

    [Fact]
    public async Task Authenticate_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("username", "invaliduser"),
            new KeyValuePair<string, string>("password", "invalidpass")
            ]);

        // Act
        var response = await client.PostAsync("/auth/Authenticate", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
