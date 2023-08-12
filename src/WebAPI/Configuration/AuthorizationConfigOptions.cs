using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.Configuration;

public class AuthorizationConfigOptions : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY, CreatePolicy());
    }
    private static AuthorizationPolicy CreatePolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new BoardAccessRequirement())
            .Build();
    }
}
