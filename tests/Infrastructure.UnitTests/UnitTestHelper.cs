using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.UnitTests;

public static class UnitTestHelper
{
    public static TrackerDbContext GetTestDbContext()
    {
        var options = new DbContextOptionsBuilder<TrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new TrackerDbContext(options);
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
