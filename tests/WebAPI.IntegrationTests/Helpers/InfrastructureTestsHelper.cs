using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.Configuration;

namespace TaskTracker.WebAPI.IntegrationTests.Helpers;

internal static class InfrastructureTestsHelper
{
    public static TrackerDbContext GetTestDbContext()
    {
        var testConnectionString = GetTestConnectionString();
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c.GetSection("ConnectionStrings")["TrackerConnection"])
            .Returns(testConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder<TrackerDbContext>();

        var dbContextConfiguration = new DbContextConfiguration(configuration.Object);
        dbContextConfiguration.ConfigureDbContext(optionsBuilder);

        return new TrackerDbContext(optionsBuilder.Options);
    }

    private static string GetTestConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<Program>()
            .Build();
        string? connectionString = configuration.GetConnectionString("TestConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Test connection string does not exist");

        return connectionString;
    }

    public static UserManager<User> GetUserManager(TrackerDbContext context)
    {
        var userStore = new UserStore<User>(context);
        var passordHasher = new PasswordHasher<User>();
        return new UserManager<User>(userStore,
            null, passordHasher, null, null, null, null, null, null);
    }

    public static RoleManager<IdentityRole> GetRoleManager(TrackerDbContext context)
    {
        var roleStore = new RoleStore<IdentityRole>(context);
        return new RoleManager<IdentityRole>(roleStore, null, null, null, null);
    }
}
