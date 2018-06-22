using Zenith.Authorization;
using Zenith.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

// dotnet aspnet-codegenerator razorpage -m Request -dc ApplicationDbContext -outDir Pages\Requests --referenceScriptLibraries
namespace Zenith.Data
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
                    Name = "Debra Garcia",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-07-20",
                    Reason = "Wedding",
                    Email = "debra@example.com",
                    Status = RequestStatus.Approved,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Thorsten Weinrich",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-06-25",
                    Reason = "Baptism",
                    Email = "thorsten@example.com",
                    Status = RequestStatus.Submitted,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Yuhong Li",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-06-22",
                    Reason = "Kid's graduation",
                    Email = "yuhong@example.com",
                    Status = RequestStatus.Submitted,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Jon Orton",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-08-21",
                    Reason = "Just because",
                    Email = "jon@example.com",
                    Status = RequestStatus.Rejected,
                    OwnerID = adminID
                },
                new Request
                {
                    Name = "Diliana Alexieva-Bosseva",
                    DateOfToday = "2018-06-20",
                    DateOfRequest = "2018-08-01",
                    Reason = "Personal",
                    Email = "diliana@example.com",
                    Status = RequestStatus.Submitted,
                    OwnerID = adminID
                }
             );
            context.SaveChanges();
        }
    }
}