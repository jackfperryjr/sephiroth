using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Sephiroth.Pages.Requests
{
    public class DeleteModel : DI_BaseModel
    {
        public DeleteModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
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
                                                     RequestOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Request = await Context.Request.FindAsync(id);

            var request = await Context
                .Request.AsNoTracking()
                .FirstOrDefaultAsync(m => m.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, request,
                                                     RequestOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Request.Remove(Request);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}