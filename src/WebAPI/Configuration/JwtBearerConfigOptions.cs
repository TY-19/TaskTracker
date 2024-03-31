using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TaskTracker.WebAPI.Configuration
{
    public class JwtBearerConfigOptions : IConfigureOptions<JwtBearerOptions>
    {
        private readonly IConfiguration _configuration;
        public JwtBearerConfigOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtBearerOptions options)
        {
            options.TokenValidationParameters = GetTokenValidationParameters();
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration?["JwtSettings:Issuer"] ?? "TaskTracker",
                ValidAudience = _configuration?["JwtSettings:Audience"] ?? "*",
                IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(
                            _configuration?["JwtSettings:SecurityKey"] 
                                ?? "defaultSecurityKeyThatHasAProperLength"))
            };
        }
    }
}
