using Fundo.Applications.WebApi;
using Fundo.Infraestructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Fundo.Services.Tests.Common
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>, IAsyncLifetime
    {
        private readonly MsSqlContainer dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("8MYgAO6ckKDb1zpBg7uV!")
            .Build();

        public async Task InitializeAsync() => await dbContainer.StartAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"FundoLoan:ConnectionString", dbContainer.GetConnectionString()}
                });
            });

            builder.ConfigureTestServices(services =>
            {
                var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<FundoLoanDbContext>));
                if (serviceDescriptor != null)
                    services.Remove(serviceDescriptor);

                services.AddDbContext<FundoLoanDbContext>(op => op.UseSqlServer(dbContainer.GetConnectionString()));
            });
        }

        public new async Task DisposeAsync() => await dbContainer.StopAsync();
    }
}
