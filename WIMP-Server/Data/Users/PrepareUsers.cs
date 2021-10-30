using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WIMP_Server.Auth.Roles;
using WIMP_Server.Models.Users;
using WIMP_Server.Options;

namespace WIMP_Server.Data.Users
{
    public static class PrepareUsers
    {
        public static void Prepare(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            PrepareRoles(serviceScope.ServiceProvider).Wait();
            PrepareDefaultUser(serviceScope.ServiceProvider).Wait();
        }

        private static async Task PrepareRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Role.AllRoles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role)
                    .ConfigureAwait(true);
                if (!roleExist)
                {
                    var createRoleResult = await roleManager.CreateAsync(new IdentityRole(role))
                        .ConfigureAwait(true);
                    if (!createRoleResult.Succeeded)
                    {
                        throw new Exception($"Couldn't prepare role: {role}");
                    }
                }
            }
        }

        private static async Task PrepareDefaultUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var defaultUser = serviceProvider.GetRequiredService<IOptions<DefaultUserOptions>>().Value;

            if (defaultUser.DisableCreate)
            {
                return;
            }

            var user = await userManager.FindByNameAsync(defaultUser.Username)
                .ConfigureAwait(true);
            if (user == null)
            {
                user = new User
                {
                    UserName = defaultUser.Username
                };

                var createUserResult = await userManager.CreateAsync(user, defaultUser.Password)
                    .ConfigureAwait(true);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception("Couldn't create default user");
                }

                var addRolesResult = await userManager.AddToRoleAsync(user, Role.Admin)
                    .ConfigureAwait(true);
                if (!addRolesResult.Succeeded)
                {
                    throw new Exception("Couldn't add default roles to user");
                }
            }
        }
    }
}