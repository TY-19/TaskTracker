using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure;
using TaskTracker.WebAPI.IntegrationTests.Helpers;

namespace TaskTracker.WebAPI.IntegrationTests.Infractructure;

public class SeedDataTests
{
    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_ApplyPendingMigrations()
    {
        TrackerDbContext context = InfrastructureTestsHelper.GetTestDbContext();
        await context.Database.EnsureDeletedAsync();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);
        int numberMigrationsBeforeExecution = (await context.Database.GetPendingMigrationsAsync()).Count();

        await seeder.SeedDefaultRolesAndUsersAsync();
        int numberMigrationsAfterExecution = (await context.Database.GetPendingMigrationsAsync()).Count();

        Assert.NotEqual(0, numberMigrationsBeforeExecution);
        Assert.Equal(0, numberMigrationsAfterExecution);
    }
    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_WorksCorrectlyIfThereAreNoPendingMigrations()
    {
        var context = InfrastructureTestsHelper.GetTestDbContext();
        await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);
        int numberMigrationsBeforeExecution = (await context.Database.GetPendingMigrationsAsync()).Count();

        await seeder.SeedDefaultRolesAndUsersAsync();
        int numberMigrationsAfterExecution = (await context.Database.GetPendingMigrationsAsync()).Count();

        Assert.Equal(0, numberMigrationsBeforeExecution);
        Assert.Equal(0, numberMigrationsAfterExecution);
    }

    private static SeedData GetSeedDataInstance(TrackerDbContext context, IConfiguration configuration)
    {
        UserManager<User> userManager = InfrastructureTestsHelper.GetUserManager(context);
        RoleManager<IdentityRole> roleManager = InfrastructureTestsHelper.GetRoleManager(context);
        return new SeedData(context, userManager, roleManager, configuration);
    }
}
