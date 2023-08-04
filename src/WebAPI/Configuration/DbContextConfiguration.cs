using Microsoft.EntityFrameworkCore;

namespace TaskTracker.WebAPI.Configuration
{
    public class DbContextConfiguration
    {
        private readonly string _connectionString;
        public DbContextConfiguration(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TrackerConnection");
        }

        public void ConfigureDbContext(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(_connectionString,
                new SqlServerConfiguration().ConfigureSqlServer);
        }
    }
}
