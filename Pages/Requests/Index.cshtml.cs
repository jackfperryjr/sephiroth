using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sephiroth.Pages.Requests
{
    public class IndexModel : DI_BaseModel
    {
        private readonly IEmailSender _emailSender;
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender) 
            : base(context, authorizationService, userManager)
        {
            _emailSender = emailSender;
        }
        public IList<Request> Request { get; set; }

        public async Task OnGetAsync()
        {
            var requests = from c in Context.Request
                           select c;

            var isAuthorized = User.IsInRole(Constants.RequestManagersRole) ||
                               User.IsInRole(Constants.RequestAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            // Only approved requests are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                requests = requests.Where(c => c.Status == RequestStatus.Approved
                                            || c.OwnerID == currentUserId);
            }

            Request = await requests.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int id, RequestStatus status)
        {
            var request = await Context.Request.FirstOrDefaultAsync(m => m.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var requestOperation = (status == RequestStatus.Approved) 
                                                       ? RequestOperations.Approve
                                                       : RequestOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, request, requestOperation);
            
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            request.Status = status;

            // Checking for which operation is being requested
            // If APPROVE or REJECT, the request is updated, saved, 
            // and a notification is sent for the operation.
            if (request.Status == RequestStatus.Approved || request.Status == RequestStatus.Rejected)
            {
                Context.Request.Update(request);
                await Context.SaveChangesAsync();
                await _emailSender.SendStatusUpdateAsync(request.Email, request.Status, request.Name, request.DateOfRequest, request.Reason);
            }

            // If it's DELETE, the request is removed, and saved.
            // No notification is sent.
            else 
            {
                Context.Request.Remove(request);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}

