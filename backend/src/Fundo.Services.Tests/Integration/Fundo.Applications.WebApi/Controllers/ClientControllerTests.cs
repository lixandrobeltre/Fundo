using Bogus;
using Fundo.Application.Dtos;
using Fundo.Services.Tests.Common;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration;

public class ClientControllerTests : BaseClassFixture
{
    public ClientControllerTests(TestWebApplicationFactory applicationFactory) : base(applicationFactory) { }

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
        this.Client.DefaultRequestHeaders.Authorization = await this.GetToken();
        var response = await Client.PostAsJsonAsync("/clients", dto);

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

        this.Client.DefaultRequestHeaders.Authorization = await this.GetToken();
        await Client.PostAsJsonAsync("/clients", dto);

        // Act
        this.Client.DefaultRequestHeaders.Authorization = await this.GetToken();
        var response = await Client.GetAsync($"/clients/{dto.Code}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(client);
        Assert.Equal(dto.Code, client.Code);
        Assert.Equal(dto.Name, client.Name);
    }
}
