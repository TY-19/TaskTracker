using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.UnitTests;

public class SeedDataTests
{
    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsUserToTheDatabase()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.True(context.Users.Any());
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsRolesToTheDatabase()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.True(context.Roles.Any());
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsUserWithDefaultName_IfNotProvidedInConfiguration()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.Equal(1, context.Users.Count());
        Assert.Equal("admin", context.Users.First().UserName);
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsUserWithProvidedCredentials_IfConfigurationContainsCredentials()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configuration = GetTestIConfigurationMock();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.Equal(1, context.Users.Count());
        Assert.Equal("TestsAdminName", context.Users.First().UserName);
        Assert.Equal("testadminemail@example.com", context.Users.First().Email);
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsRolesWithDefaultNames()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.Equal(3, context.Roles.Count());
        Assert.True(context.Roles.Any(r => r.Name == DefaultRolesNames.DEFAULT_ADMIN_ROLE));
        Assert.True(context.Roles.Any(r => r.Name == DefaultRolesNames.DEFAULT_MANAGER_ROLE));
        Assert.True(context.Roles.Any(r => r.Name == DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE));
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_DoesNotAddUser_IfDatabaseAlreadyContainsUsers()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        context.Users.Add(new User() { UserName = "ExistedUser" });
        var configuration = new Mock<IConfiguration>();
        var seeder = GetSeedDataInstance(context, configuration.Object);

        await seeder.SeedDefaultRolesAndUsersAsync();

        Assert.Equal(1, context.Users.Count());
        Assert.Equal("ExistedUser", context.Users.First().UserName);
    }

    [Fact]
    public async Task SeedDefaultRolesAndUsersAsync_AddsNewAdmin_IfConfigurationParameterSetNewAdminIsTrue()
    {
        TrackerDbContext context = UnitTestHelper.GetTestDbContext();
        var configurationEmpty = new Mock<IConfiguration>();
        var configurationFilled = GetTestIConfigurationMock();
        configurationFilled.Setup(c => c["SetNewAdmin"]).Returns("true");
        var seeder1 = GetSeedDataInstance(context, configurationEmpty.Object);
        var seeder2 = GetSeedDataInstance(context, configurationFilled.Object);

        await seeder1.SeedDefaultRolesAndUsersAsync();
        await seeder2.SeedDefaultRolesAndUsersAsync();

        Assert.Equal(2, context.Users.Count());
        Assert.True(context.Users.Any(u => u.UserName == "admin"));
        Assert.True(context.Users.Any(u => u.UserName == "TestsAdminName"));
    }

    private static SeedData GetSeedDataInstance(TrackerDbContext context, IConfiguration configuration)
    {
        UserManager<User> userManager = UnitTestHelper.GetUserManager(context);
        RoleManager<IdentityRole> roleManager = UnitTestHelper.GetRoleManager(context);
        return new SeedData(context, userManager, roleManager, configuration);
    }

    private static Mock<IConfiguration> GetTestIConfigurationMock()
    {
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["DefaultCredentials:Admin:Name"])
            .Returns("TestsAdminName");
        configuration.Setup(c => c["DefaultCredentials:Admin:Email"])
            .Returns("testadminemail@example.com");
        configuration.Setup(c => c["DefaultCredentials:Admin:Password"])
            .Returns("TestAdminPass1234");
        return configuration;
    }
}