namespace BarakaBg.Web.Areas.Administration.Security
{
    using System.Threading.Tasks;

    using BarakaBg.Common;
    using Microsoft.AspNetCore.Authorization;

    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            if (context.User.IsInRole(GlobalConstants.SuperAdministratorName))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}