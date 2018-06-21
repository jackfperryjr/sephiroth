using Zenith.Authorization;
using Zenith.Data;
using Zenith.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Zenith.Pages.Requests
{
    public class DetailsModel : DI_BaseModel
    {
        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager) 
            : base(context, authorizationService, userManager)
        {
        }

        public Request Request { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Request = await Context.Request.FirstOrDefaultAsync(m => m.RequestId == id);

            if (Request == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, RequestStatus status)
        {
             var request = await Context.Request.FirstOrDefaultAsync(
                                                       m => m.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var requestOperation = (status == RequestStatus.Approved) 
                                                       ? RequestOperations.Approve
                                                       : RequestOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, request,
                                        requestOperation);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            request.Status = status;
            Context.Request.Update(request);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}