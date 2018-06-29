using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Sephiroth.Pages.Requests
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
        private readonly IEmailSender _emailSender;

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


            // *** Having trouble with this one. ***    
            //await _emailSender.SendStatusUpdateAsync(request.Email, request.Status);

            return RedirectToPage("./Index");
        }
    }
}