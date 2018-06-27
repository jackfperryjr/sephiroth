using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;

namespace Sephiroth.Pages.Requests
{
    public class CreateModel : DI_BaseModel
    {

        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            Request = new Request
            {
                Name = "",
                DateOfToday = DateTime.Now.ToString("yyyy-dd-MM"),
                DateOfRequest = "",
                Reason = "",
                Email = UserManager.GetUserName(User)
            };

            return Page();
        }

        [BindProperty]
        public Request Request { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Request.OwnerID = UserManager.GetUserId(User);

            // requires using Sephiroth.Authorization;
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                        User, Request,
                                                        RequestOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Request.Add(Request);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}