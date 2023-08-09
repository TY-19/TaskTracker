using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using TaskTracker.WebAPI.Configuration;

namespace TaskTracker.WebAPI.UnitTests.Configuration;

public class JwtBearerConfigOptionsTests
{
    [Fact]
    public void Configure_DoesntThrowExceptions_IfConfigurationDoesntContainJwtSettings()
    {
        var configuration = new Mock<IConfiguration>();
        var jwtBearerConfigOptions = new JwtBearerConfigOptions(configuration.Object);

        var exception = Record.Exception(() => jwtBearerConfigOptions.Configure(new JwtBearerOptions()));

        Assert.Null(exception);
    }
    [Fact]
    public void Configure_DoesntThrowExceptions_IfConfigurationWasNotProvided()
    {
        var jwtBearerConfigOptions = new JwtBearerConfigOptions(null!);

        var exception = Record.Exception(() => jwtBearerConfigOptions.Configure(new JwtBearerOptions()));

        Assert.Null(exception);
    }
}
