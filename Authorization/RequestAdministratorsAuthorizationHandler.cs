using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;

namespace Sephiroth.Authorization
{
    public class RequestAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Request>
    {
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement, 
                                     Request resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrator can do anything.
            if (context.User.IsInRole(Constants.RequestAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}