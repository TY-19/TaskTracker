using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TaskTracker.Application.Interfaces;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.UnitTests.Configuration.AuthorizationHandlers;

internal class BoardAccessAuthorizationHandlerTestsHelper : BoardAccessAuthorizationHandler
{
    public BoardAccessAuthorizationHandlerTestsHelper(IUserService userService,
        IHttpContextAccessor httContextAccessor) : base(userService, httContextAccessor)
    {
    }

    public async Task TestHandleRequirementAsync(AuthorizationHandlerContext context,
        BoardAccessRequirement requirement)
    {
        await base.HandleRequirementAsync(context, requirement);
    }
}
