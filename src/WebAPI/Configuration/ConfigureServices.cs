using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.Configuration;
public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TrackerDbContext>(
            new DbContextConfiguration(configuration).ConfigureDbContext);

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<TrackerDbContext>();
        services.ConfigureOptions<IdentityConfigOptions>();

        services.AddAuthentication()
            .AddJwtBearer(new JwtBearerConfigOptions(configuration).Configure);
        services.ConfigureOptions<AuthenticationConfigOptions>();
        services.AddAuthorization();
        services.ConfigureOptions<AuthorizationConfigOptions>();


        services.AddControllers();
        services.ConfigureOptions<JsonConfigOptions>();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerConfigOptions>();

        services.AddCors();
        services.ConfigureOptions<CorsConfigOptions>();

        services.AddScoped<ITrackerDbContext, TrackerDbContext>();
        services.AddScoped<SeedData>();
        services.AddScoped<JwtHandlerService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IStageService, StageService>();
        services.AddScoped<ISubpartService, SubpartService>();

        services.AddScoped<IAuthorizationHandler, BoardAccessAuthorizationHandler>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddSingleton<IValidationService, ValidationService>();
        services.AddSingleton(new AutomapperConfiguration().GetMapper());

    }
}
