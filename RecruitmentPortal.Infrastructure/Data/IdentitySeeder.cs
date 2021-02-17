using Microsoft.AspNetCore.Identity;
using RecruitmentPortal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Infrastructure.Data
{
    public class IdentitySeeder
    {
        public static void Seed(RecruitmentPortalDbContext dbContext, RoleManager<IdentityRole> roleManager,
           UserManager<ApplicationUser> userManager)
        {

            // Create default Users (if there are none)
            if (!dbContext.Users.Any())
            {
                CreateUsers(dbContext, roleManager, userManager)
                    .GetAwaiter()
                    .GetResult();
            }
        }


        private static async Task CreateUsers(
   RecruitmentPortalDbContext dbContext,
   RoleManager<IdentityRole> roleManager,
   UserManager<ApplicationUser> userManager)
        {
            string role_Administrator = "Admin";
            string role_interviewer = "Interviewer";
          

            //Create Roles (if they doesn't exist yet)
            if (!await roleManager.RoleExistsAsync(role_Administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(role_Administrator));
            }
            if (!await roleManager.RoleExistsAsync(role_interviewer))
            {
                await roleManager.CreateAsync(new IdentityRole(role_interviewer));
            }


            // Create the "Admin" ApplicationUser account
            var user_Admin = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@recruitmentportal.com",
                position = "Head of Recruitment",
                skype_id ="hr@skypeid"

            };


            // Insert "Admin" into the Database and assign the "Administrator" 
            if (await userManager.FindByIdAsync(user_Admin.Id) == null)
            {
                await userManager.CreateAsync(user_Admin, "Pass4Admin");
                await userManager.AddToRoleAsync(user_Admin, role_Administrator);
                // Remove Lockout and E-Mail confirmation.
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
