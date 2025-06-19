using FluentValidation;
using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Application.Validators;
using Fundo.Applications.WebApi.Models;
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

            var corsOptions = new Cors();
            configuration.GetSection("Cors").Bind(corsOptions);

            services.AddCors(options =>
            {
                options.AddPolicy(nameof(Cors), builder =>
                {
                    builder.WithOrigins(corsOptions.AllowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors(nameof(Cors));
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
