using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fundo.Infraestructure.Data
{
    public class FundoLoanDbContextFactory : IDesignTimeDbContextFactory<FundoLoanDbContext>
    {
        public FundoLoanDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FundoLoanDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=TempDb;Trusted_Connection=true;TrustServerCertificate=true;");

            return new FundoLoanDbContext(optionsBuilder.Options);
        }
    }
}
