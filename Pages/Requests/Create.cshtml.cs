using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Sephiroth.Services;

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

        [BindProperty]
        #pragma warning disable 108
        public Request Request { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public string ReturnUrl { get; set; }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            Request = new Request
            {
                Name = "",
                DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                DateOfRequest = DateTime.Now.ToString("MM/dd/yyyy"),
                Reason = "",
                Email = UserManager.GetUserName(User)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
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

            // As noted in the Extensions file, this needs to be done dynamically.
            // But for now...
            await _emailSender.SendAdminUpdateAsync(Request.Name, Request.DateOfRequest, Request.Reason);
            await _emailSender.SendStatusUpdateAsync(Request.Email, Request.Status, Request.Name, Request.DateOfRequest, Request.Reason);

            // This isn't displaying; I don't know why.
            StatusMessage = "Request submitted. Check your email for the details and approval/rejection.";
            return RedirectToPage(returnUrl);
        }
    }
}