using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace TaskTracker.WebAPI.Configuration
{
    public class CorsConfigOptions : IConfigureOptions<CorsOptions>
    {
        private readonly IConfiguration _configuration;
        public CorsConfigOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Configure(CorsOptions options)
        {
            options.AddPolicy(name: "ConfiguredPolicy",
                cfg =>
                {
                    cfg.AllowAnyHeader();
                    cfg.AllowAnyMethod();
                    cfg.WithOrigins(_configuration["AllowedCORS"] ?? "*");
                }
            );
        }
    }
}
