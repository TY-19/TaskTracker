using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Security.Principal;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.UnitTests.Configuration.AuthorizationHandlers;

public class BoardAccessAuthorizationHandlerTests
{
    private bool authorizationSucceeded;
    public BoardAccessAuthorizationHandlerTests()
    {
        authorizationSucceeded = false;
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfBoardIsAccessibleToUser()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        const string boardId = "2";
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor(boardId);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfUserIsAdmin()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), true, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor(null);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfUserIsManager()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, true);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor(null);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfRouteDoesNotContainBoardIdSegment()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor("someValue", "someKey");

        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfBoardIdInTheRouteIsNotANumber()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor("boardName");

        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_AllowsAuthorization_IfHttpContextIsInaccessible()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 2, 3 } };
        IUserService userService = GetUserService(model);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(x => x.HttpContext).Returns((HttpContext?)null);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService,
            httpContextAccessor.Object);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.True(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_DeniesAuthorization_IfBoardIsInaccessibleToUser()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(GetIIdentity(), false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        var model = new UserProfileModel() { BoardsIds = new List<int>() { 1, 3 } };
        IUserService userService = GetUserService(model);
        const string boardId = "2";
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor(boardId);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.False(authorizationSucceeded);
    }
    [Fact]
    public async Task HandleRequirementAsync_DeniesAuthorization_IfUserIsNotAuthenticated()
    {
        ClaimsPrincipal user = GetClaimsPrincipal(null, false, false);
        AuthorizationHandlerContext authorizationContext = GetAuthorizationHandlerContext(user);
        IUserService userService = GetUserService(null);
        const string boardId = "2";
        IHttpContextAccessor httpContextAccessor = GetHttpContextAccessor(boardId);
        var handler = new BoardAccessAuthorizationHandlerTestsHelper(userService, httpContextAccessor);

        await handler.TestHandleRequirementAsync(authorizationContext, new BoardAccessRequirement());

        Assert.False(authorizationSucceeded);
    }

    private static IIdentity GetIIdentity()
    {
        var identity = new Mock<IIdentity>();
        identity.SetupGet(i => i.Name).Returns("Test");
        return identity.Object;
    }
    private static ClaimsPrincipal GetClaimsPrincipal(IIdentity? identity = null,
        bool isAdmin = false, bool isManager = false)
    {
        var userMock = new Mock<ClaimsPrincipal>();
        userMock.SetupGet(u => u.Identity).Returns(identity);
        userMock.Setup(u => u.IsInRole(DefaultRolesNames.DEFAULT_ADMIN_ROLE)).Returns(isAdmin);
        userMock.Setup(u => u.IsInRole(DefaultRolesNames.DEFAULT_MANAGER_ROLE)).Returns(isManager);
        return userMock.Object;
    }
    private AuthorizationHandlerContext GetAuthorizationHandlerContext(ClaimsPrincipal user)
    {
        var authorizationContext = new Mock<AuthorizationHandlerContext>(
            new List<IAuthorizationRequirement>() { new BoardAccessRequirement() },
            user, null);
        authorizationContext.Setup(a => a.User).Returns(user);
        authorizationContext.Setup(a => a.Succeed(It.IsAny<BoardAccessRequirement>()))
            .Callback(() => authorizationSucceeded = true);
        return authorizationContext.Object;
    }
    private static IUserService GetUserService(UserProfileModel? userProfileModel)
    {
        var userService = new Mock<IUserService>();
        userService.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(userProfileModel);
        return userService.Object;
    }

    private static IHttpContextAccessor GetHttpContextAccessor(object? routeValue, string routeKey = "boardId")
    {
        var routeValueFeature = new Mock<IRouteValuesFeature>();
        routeValueFeature.Setup(r => r.RouteValues).Returns(new RouteValueDictionary() { { routeKey, routeValue } });
        var features = new Mock<IFeatureCollection>();
        features.Setup(f => f.Get<IRouteValuesFeature>()).Returns(routeValueFeature.Object);
        var httpContext = new Mock<HttpContext>();
        httpContext.SetupGet(h => h.Features).Returns(features.Object);
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
        return httpContextAccessor.Object;
    }
}
