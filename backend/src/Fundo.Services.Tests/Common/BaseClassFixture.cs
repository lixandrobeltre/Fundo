using Bogus;
using Fundo.Domain.Models;
using Fundo.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Common
{
    public abstract class BaseClassFixture : IClassFixture<TestWebApplicationFactory>
    {
        public string UsernameTest { get; init; } = "user_testing";
        public string PasswordTest { get; init; } = "u$3r_43$4ln9!";

        protected HttpClient Client { get; init; }
        protected IServiceScope ServiceScope { get; init; }

        protected BaseClassFixture(TestWebApplicationFactory applicationFactory)
        {
            Client = applicationFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true,
                BaseAddress = new Uri("https://localhost:5001")
            });

            ServiceScope = applicationFactory.Services.CreateScope();
            Seed();
        }

        protected async Task<AuthenticationHeaderValue> GetToken()
        {
            var authResponse = await this.Client.PostAsync("/auth/authenticate",
                new FormUrlEncodedContent([
                    new KeyValuePair<string, string>("username", this.UsernameTest),
                    new KeyValuePair<string, string>("password", this.PasswordTest)
                    ]));

            authResponse.EnsureSuccessStatusCode();

            var response = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();

            return new AuthenticationHeaderValue(response.Token_type, response.Access_token);
        }

        void Seed()
        {
            var db = ServiceScope.ServiceProvider.GetRequiredService<FundoLoanDbContext>();
            db.Database.EnsureCreated();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(this.PasswordTest);
            var faker = new Faker();

            db.Users.Add(new User
            {
                Code = faker.Random.AlphaNumeric(7).ToUpper(),
                Name = faker.Name.FullName(),
                Username = this.UsernameTest,
                PasswordHash = passwordHash,
                Status = true,
                RowId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });

            db.SaveChanges();
        }

        public void Dispose()
        {
            Client.Dispose();
            ServiceScope.Dispose();
        }
    }
}
