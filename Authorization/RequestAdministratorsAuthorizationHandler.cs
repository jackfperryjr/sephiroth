using System.Threading.Tasks;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Sephiroth.Authorization
{
    public class RequestAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Request>
    {
        private readonly IEmailSender _emailSender;

        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement, 
                                     Request resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.RequestAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            //await _emailSender.SendStatusUpdateAsync(Email);
            return Task.CompletedTask;
        }
    }
}