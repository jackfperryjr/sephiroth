using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sephiroth.Authorization;
using Sephiroth.Models;

namespace Sephiroth.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes we are seeding 2 users both with the same password.
                // The password is set with the following command:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything
                var adminID = await EnsureUser(serviceProvider, testUserPw, "sephiroth@sephiroth.com");
                await EnsureRole(serviceProvider, adminID, Constants.RequestAdministratorsRole);

                // Allowed user can view and edit requests that they create
                // (They are not assigned a role.)
                var mentorID = await EnsureUser(serviceProvider, testUserPw, "mentor@sephiroth.com");

                SeedDB(context, adminID, mentorID);
            }
        }
      
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string mentorID, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(mentorID);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        public static void SeedDB(ApplicationDbContext context, string adminID, string mentorID)
        {
            if (context.Request.Any())
            {
                // Database has been seeded.
                return;   
            }

            // Below is for seeding purposes only.
            // The Final Fantasy character's requests are tagged with adminID.
            // There are two requests tagged with mentorID.
            context.Request.AddRange(
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "I need to save the world",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Aerith Gainsborough",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "Sephiroth killed me!",
                    Email = "a.gainsborough@fantasy.com",
                    Status = RequestStatus.Approved,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Tifa Lockhart",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "I'm in love with Cloud",
                    Email = "tifa.lockhart@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Yuffie Kisaragi",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "Materia!",
                    Email = "materia.hunter@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "Family reunion",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "I need to figure out who I really am",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Mentor",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/20/2018",
                    Reason = "Not doing Code Louisville stuff",
                    Email = "mentor@sephiroth.com",
                    Status = RequestStatus.Submitted,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Mentor",
                    DateOfToday = DateTime.Now.ToString("MM/dd/yyyy"),
                    DateOfRequest = "07/27/2018",
                    Reason = "Checking Code Louisville projects",
                    Email = "mentor@sephiroth.com",
                    Status = RequestStatus.Approved,
                    OwnerID = mentorID
                }
             );
            context.SaveChanges();
        }
    }
}