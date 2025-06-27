using FluentValidation;
using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Application.Validators;
using Fundo.Applications.WebApi.Models;
using Fundo.Applications.WebApi.Services;
using Fundo.Infraestructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.MSSqlServer(
                connectionString: configuration.GetSection("FundoLoan:ConnectionString").Value,
                sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
                .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddValidatorsFromAssemblyContaining<LoanValidator>();

            services.AddDbContext<FundoLoanDbContext>(op => op.UseSqlServer(
                configuration.GetSection("FundoLoan:ConnectionString").Value,
                options => options.MigrationsAssembly("Fundo.Infraestructure")));

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogin, LoginService>();

            services.Configure<JwtOption>(configuration.GetSection("Jwt"));
            services.AddHttpContextAccessor();

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


            var jwtSettings = configuration.GetSection("Jwt").Get<JwtOption>();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("SecurityKey").Value))
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<Middleware.ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseCors(nameof(Cors));
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
