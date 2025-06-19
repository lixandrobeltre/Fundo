using FluentValidation;
using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Application.Validators;
using Fundo.Infraestructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddValidatorsFromAssemblyContaining<LoanValidator>();

            services.AddDbContext<FundoLoanDbContext>(op => op.UseSqlServer(configuration.GetSection("FundoLoan:ConnectionString").Value));

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserService, UserService>();         
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
