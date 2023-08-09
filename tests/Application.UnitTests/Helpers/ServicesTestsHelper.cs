using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Helpers;

internal static class ServicesTestsHelper
{
    public static TestDbContext GetTestDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    public static Mock<UserManager<User>> GetMockUserManager(TestDbContext context)
    {
        var userStore = new UserStore<User>(context);
        var passwordHasher = new PasswordHasher<User>();
        var options = new OptionsWrapper<IdentityOptions>(GetIdentityOptions());
        var passwordValidators = new List<IPasswordValidator<User>>() { new PasswordValidator<User>() };
        var logger = new Logger<UserManager<User>>(new LoggerFactory());

        return new Mock<UserManager<User>>(MockBehavior.Loose, userStore,
            options, passwordHasher, null, passwordValidators, null, null, null, logger);
    }

    public static UserManager<User> GetUserManager(TestDbContext context)
    {
        var userStore = new UserStore<User>(context);
        var passwordHasher = new PasswordHasher<User>();
        var options = new OptionsWrapper<IdentityOptions>(GetIdentityOptions());
        var passwordValidators = new List<IPasswordValidator<User>>() { new PasswordValidator<User>() };
        var logger = new Logger<UserManager<User>>(new LoggerFactory());

        return new UserManager<User>(userStore,
            options, passwordHasher, null, passwordValidators, null, null, null, logger);
    }

    public static RoleManager<IdentityRole> GetRoleManager(TestDbContext context)
    {
        var roleStore = new RoleStore<IdentityRole>(context);
        return new RoleManager<IdentityRole>(roleStore, null, null, null, null);
    }

    public static IMapper GetMapper()
    {
        return new MapperConfiguration(cfg =>
            cfg.AddProfile<TestAutomapperProfile>()).CreateMapper();
    }

    private static IdentityOptions GetIdentityOptions()
    {
        IdentityOptions options = new();
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        return options;
    }
}
