using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mapping;
using TaskTracker.Application.Models;
using TaskTracker.Application.Services;
using TaskTracker.Application.Validators;
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
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IStageService, StageService>();
        services.AddScoped<ISubpartService, SubpartService>();

        services.AddSingleton(new MapperConfiguration(cfg =>
            cfg.AddProfile<AutomapperProfile>()).CreateMapper());

        services.AddScoped<IValidator<RegistrationRequestModel>, RegistrationRequestModelValidator>();
    }
}
