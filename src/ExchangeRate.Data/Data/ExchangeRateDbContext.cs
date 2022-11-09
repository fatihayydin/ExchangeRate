using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Data.Data
{
    public class ExchangeRateDbContext : DbContext
    {
        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerApiLog> CustomerApiLogs { get; set; }
        public DbSet<CustomerApiKey> CustomerApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
