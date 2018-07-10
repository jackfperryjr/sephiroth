using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;
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
        private readonly IEmailSender _emailSender;

        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
            : base(context, authorizationService, userManager)
        {
            _emailSender = emailSender;
        }

        public IActionResult OnGet()
        {
            Request = new Request
            {
                Name = "",
                DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
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
            await _emailSender.SendStatusUpdateAsync(Request.Email, Request.Status);

            return RedirectToPage("./Index");
        }
    }
}