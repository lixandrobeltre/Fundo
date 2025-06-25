using Bogus;
using Fundo.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration;

public class ClientControllerTests : IClassFixture<WebApplicationFactory<Applications.WebApi.Startup>>
{
    private readonly HttpClient _client;

    public ClientControllerTests(WebApplicationFactory<Applications.WebApi.Startup> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task CreateClient_ReturnsCreated()
    {
        // Arrange
        var faker = new Faker();
        var dto = new CreateClientDto
        {
            Code = faker.Random.AlphaNumeric(6).ToUpper(),
            Identification = faker.Random.ReplaceNumbers("#########"),
            Name = faker.Name.FullName(),
            Email = faker.Internet.Email()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/clients", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(client);
        Assert.Equal(dto.Code, client.Code);
        Assert.Equal(dto.Name, client.Name);
    }

    [Fact]
    public async Task GetClient_ReturnsOk()
    {
        // Arrange: Ensure the client exists
        var faker = new Faker();
        var dto = new CreateClientDto
        {
            Code = faker.Random.AlphaNumeric(7).ToUpper(),
            Identification = faker.Random.ReplaceNumbers("#########"),
            Name = faker.Name.FullName(),
            Email = faker.Internet.Email()
        };
        await _client.PostAsJsonAsync("/clients", dto);

        // Act
        var response = await _client.GetAsync($"/clients/{dto.Code}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(client);
        Assert.Equal(dto.Code, client.Code);
        Assert.Equal(dto.Name, client.Name);
    }

}
