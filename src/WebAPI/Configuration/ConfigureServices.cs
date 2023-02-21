using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;

namespace TaskTracker.WebAPI.Configuration;
public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TrackerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TrackerConnection")));
        services.AddScoped<ITrackerDbContext>(provider => provider.GetRequiredService<TrackerDbContext>());

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<TrackerDbContext>();
        services.ConfigureOptions<IdentityConfigOptions>();

        services.AddAuthentication()
            .AddJwtBearer(new JwtBearerConfigOptions(configuration).Configure);
        services.ConfigureOptions<AuthenticationConfigOptions>();

        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerConfigOptions>();

        services.AddCors();
        services.ConfigureOptions<CorsConfigOptions>();

        services.AddScoped<SeedData>();
        services.AddScoped<JwtHandlerService>();
        services.AddScoped<IAccountService, AccountService>();

        services.AddSingleton(new MapperConfiguration(cfg =>
            cfg.AddProfile<AutomapperProfile>()).CreateMapper());
    }
}
