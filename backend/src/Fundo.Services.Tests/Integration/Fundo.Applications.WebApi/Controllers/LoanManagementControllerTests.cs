using Bogus;
using Fundo.Application.Dtos;
using Fundo.Applications.WebApi;
using Fundo.Applications.WebApi.Models;
using Fundo.Services.Tests.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration;

record class ResponseLoan(string Id);

public class LoanManagementControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public LoanManagementControllerTests(WebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    private async Task<string> CreateClientAsync()
    {
        var clientDto = FakerData.GenerateClientDto();

        var response = await _client.PostAsJsonAsync("/clients", clientDto);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<ClientDto>();
        return clientDto.Code;
    }

    private async Task<string> CreateLoanAsync(string clientCode)
    {
        var loanDto = FakerData.GenerateLoanDto(clientCode);
        var response = await _client.PostAsJsonAsync("/loans", loanDto);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<ResponseLoan>();
        return created.Id;
    }


    [Fact]
    public async Task GetBalances_ShouldReturnExpectedResult()
    {
        var response = await _client.GetAsync("/loans");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetLoanById_ReturnsOk()
    {
        // Arrange
        var clientCode = await CreateClientAsync();
        var loanDto = FakerData.GenerateLoanDto(clientCode);

        var createResponse = await _client.PostAsJsonAsync("/loans", loanDto);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseLoan>();

        // Act
        var response = await _client.GetAsync($"/loans/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MakePayment_AfterLoanCreation_ShouldReturnOkOrBadRequest()
    {
        // Arrange: create client and loan
        var clientCode = await CreateClientAsync();
        var loanCode = await CreateLoanAsync(clientCode);

        var paymentFaker = new Faker<PaymentRequest>()
            .CustomInstantiator(f => new PaymentRequest(f.Finance.Amount(10, 1000), f.Lorem.Sentence()));

        var paymentRequest = paymentFaker.Generate();

        // Act
        var response = await _client.PostAsJsonAsync($"/loans/{loanCode}/payment", paymentRequest);

        // Assert: should be OK or BadRequest depending on business logic
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.OK ||
            response.StatusCode == System.Net.HttpStatusCode.BadRequest
        );
    }
}
