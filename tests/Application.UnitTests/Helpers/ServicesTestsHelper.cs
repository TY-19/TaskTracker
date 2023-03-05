using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using AutoMapper;
using TaskTracker.Application.Mapping;

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

    public static UserManager<User> GetUserManager(TestDbContext context)
    {
        var userStore = new UserStore<User>(context);
        var passordHasher = new PasswordHasher<User>();
        return new UserManager<User>(userStore,
            null, passordHasher, null, null, null, null, null, null);
    }

    public static RoleManager<IdentityRole> GetRoleManager(TestDbContext context)
    {
        var roleStore = new RoleStore<IdentityRole>(context);
        return new RoleManager<IdentityRole>(roleStore, null, null, null, null);
    }

    public static IMapper GetMapper()
    {
        return new MapperConfiguration(cfg =>
            cfg.AddProfile<AutomapperProfile>()).CreateMapper();
    }
}
