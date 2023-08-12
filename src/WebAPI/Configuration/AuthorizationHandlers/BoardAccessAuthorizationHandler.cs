using Microsoft.AspNetCore.Authorization;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;

namespace TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

public class BoardAccessAuthorizationHandler : AuthorizationHandler<BoardAccessRequirement>
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httContextAccessor;
    public BoardAccessAuthorizationHandler(IUserService userService,
        IHttpContextAccessor httContextAccessor)
    {
        _userService = userService;
        _httContextAccessor = httContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        BoardAccessRequirement requirement)
    {
        string userName = context.User.Identity?.Name ?? "";
        var userProfileModel = await _userService.GetUserByNameOrIdAsync(userName);
        if (userProfileModel == null)
            return;

        if (context.User.IsInRole(DefaultRolesNames.DEFAULT_ADMIN_ROLE)
            || context.User.IsInRole(DefaultRolesNames.DEFAULT_MANAGER_ROLE))
        {
            context.Succeed(requirement);
            return;
        }

        object? boardIdObject = _httContextAccessor.HttpContext?.GetRouteValue("boardId");
        if (int.TryParse(boardIdObject?.ToString(), out var boardId))
        {
            if (userProfileModel.BoardsIds.Any(id => id == boardId))
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            context.Succeed(requirement);
        }
    }
}
