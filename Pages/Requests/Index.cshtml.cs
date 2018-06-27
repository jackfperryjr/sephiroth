using Sephiroth.Authorization;
using Sephiroth.Data;
using Sephiroth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sephiroth.Pages.Requests
{
    public class IndexModel : DI_BaseModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
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
    }
}