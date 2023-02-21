using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace TaskTracker.WebAPI.Configuration
{
    public class IdentityConfigOptions : IConfigureOptions<IdentityOptions>
    {
        public void Configure(IdentityOptions options)
        {
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
        }
    }
}
