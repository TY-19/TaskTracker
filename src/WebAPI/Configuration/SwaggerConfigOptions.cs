using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskTracker.WebAPI.Configuration
{
    public class SwaggerConfigOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", GetVersionInfo());
            options.AddSecurityDefinition("Bearer", GetJwtSecurityScheme());
            options.AddSecurityRequirement(GetSecurityRequirement());
        }

        private OpenApiInfo GetVersionInfo()
        {
            return new OpenApiInfo
            {
                Title = "Task Tracker",
                Version = "v1"
            };
        }
        private OpenApiSecurityScheme GetJwtSecurityScheme()
        {
            return new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            };
        }
        private OpenApiSecurityRequirement GetSecurityRequirement()
        {
            return new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };
        }
    }
}
