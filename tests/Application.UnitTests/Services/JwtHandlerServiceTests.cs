using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using TaskTracker.Application.Services;
using TaskTracker.Application.UnitTests.Helpers;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Services;

public class JwtHandlerServiceTests
{
    [Fact]
    public async Task GetTokenAsync_ReturnsToken()
    {
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["JwtSettings:SecurityKey"]).Returns("testkey");
        var context = ServicesTestsHelper.GetTestDbContext();
        var user = new User() { UserName = "TestName", Email = "testemail@example.com" };
        var service = new JwtHandlerService(configuration.Object, 
            ServicesTestsHelper.GetUserManager(context));

        var token = await service.GetTokenAsync(user);
        
        Assert.NotNull(token);
        Assert.IsType<JwtSecurityToken>(token);
    }
    [Fact]
    public async Task GetTokenAsync_ReturnsTokenWithTheCorrectDetails()
    {

    }
}
