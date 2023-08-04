using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TaskTracker.WebAPI.Configuration
{
    public class SqlServerConfiguration
    {
        public void ConfigureSqlServer(SqlServerDbContextOptionsBuilder builder)
        {
            builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    }
}
