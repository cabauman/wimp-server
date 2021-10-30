using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WIMP_Server.Auth.Policies.Requirements;
using WIMP_Server.Auth.Roles;

namespace WIMP_Server.Auth.Policies.Handlers
{
    public class OnlyAdminsAuthorizationHandler : AuthorizationHandler<OnlyAdminsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyAdminsRequirement requirement)
        {
            if (context.User.IsInRole(Role.Admin))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}