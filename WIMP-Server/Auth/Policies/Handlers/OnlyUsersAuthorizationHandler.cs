using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WIMP_Server.Auth.Policies.Requirements;
using WIMP_Server.Auth.Roles;

namespace WIMP_Server.Auth.Policies.Handlers;

public class OnlyUsersAuthorizationHandler : AuthorizationHandler<OnlyUsersRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyUsersRequirement requirement)
    {
        if (context.User.IsInRole(Role.User) || context.User.IsInRole(Role.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
