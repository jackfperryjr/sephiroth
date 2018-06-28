using Sephiroth.Authorization;
using Sephiroth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

// dotnet aspnet-codegenerator razorpage -m Request -dc ApplicationDbContext -outDir Pages\Requests --referenceScriptLibraries
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

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@example.com");
                await EnsureRole(serviceProvider, adminID, Constants.RequestAdministratorsRole);

                // allowed user can create and edit requests that they create
                var uid = await EnsureUser(serviceProvider, testUserPw, "manager@example.com");
                await EnsureRole(serviceProvider, uid, Constants.RequestManagersRole);

                SeedDB(context, adminID);
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
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Request.Any())
            {
                return;   // DB has been seeded
            }

            context.Request.AddRange(
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-07-20",
                    Reason = "I need to save the world",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Aerith Gainsborough",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-06-25",
                    Reason = "Sephiroth killed me!",
                    Email = "a.gainsborough@fantasy.com",
                    Status = RequestStatus.Approved,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Tifa Lockhart",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-06-22",
                    Reason = "I'm in love with Cloud",
                    Email = "tifa.lockhart@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Yuffie Kisaragi",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-08-21",
                    Reason = "Materia!",
                    Email = "materia.hunter@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-08-01",
                    Reason = "Family reunion",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Cloud Strife",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-08-01",
                    Reason = "I need to figure out who I really am",
                    Email = "cloud.strife@fantasy.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                }
             );
            context.SaveChanges();
        }
    }
}