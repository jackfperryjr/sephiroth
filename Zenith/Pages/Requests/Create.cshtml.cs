using Zenith.Authorization;
using Zenith.Data;
using Zenith.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Zenith.Pages.Requests
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
                Name = "Rick Johns",
                DateOfToday = "06/20/2018",
                DateOfRequest = "07/20/2018",
                Reason = "Wedding",
                Email = "rick@example.com"
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

            // requires using Zenith.Authorization;
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