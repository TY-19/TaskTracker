using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;

namespace TaskTracker.WebAPI.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration) 
    {
        services.AddDbContext<TrackerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TrackerConnection")));
        services.AddScoped<ITrackerDbContext>(provider => provider.GetRequiredService<TrackerDbContext>());

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<TrackerDbContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(
                            configuration["JwtSettings:SecurityKey"] ?? "defaultKey"))
                };
            });

        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Task Tracker",
                Version = "v1"
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            });
        });
        services.AddScoped<SeedData>();
        services.AddScoped<JwtHandlerService>();
        services.AddScoped<IAccountService, AccountService>();

        services.AddCors(options => options.AddPolicy(name: "ConfiguredPolicy",
            cfg =>
            {
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
                cfg.WithOrigins(configuration["AllowedCORS"] ?? "*");
            }
            ));
        return services;
    }
}
