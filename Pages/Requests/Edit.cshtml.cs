using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;

namespace Sephiroth.Pages.Requests
{
    public class EditModel : DI_BaseModel
    {
        public EditModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        #pragma warning disable 108
        public Request Request { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Request = await Context.Request.FirstOrDefaultAsync(
                                                 m => m.RequestId == id);

            if (Request == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Request,
                                                      RequestOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Request from DB to get OwnerID.
            var request = await Context
                .Request.AsNoTracking()
                .FirstOrDefaultAsync(m => m.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, request,
                                                     RequestOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Request.OwnerID = request.OwnerID;

            Context.Attach(Request).State = EntityState.Modified;

            if (request.Status == RequestStatus.Approved)
            {
                // If the request is updated after approval, 
                // and the user cannot approve,
                // set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        request,
                                        RequestOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    request.Status = RequestStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool RequestExists(int id)
        {
            return Context.Request.Any(e => e.RequestId == id);
        }
    }
}