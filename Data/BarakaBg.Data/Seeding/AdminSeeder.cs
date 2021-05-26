﻿namespace BarakaBg.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class AdminSeeder : ISeeder
    {
        private string adminUserName = "admin@localhost.com";

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await this.SeedRoleAsync(userManager, GlobalConstants.AdministratorRoleName);
        }

        private async Task SeedRoleAsync(UserManager<ApplicationUser> userManager, string administratorRoleName)
        {
            if (userManager.FindByNameAsync(this.adminUserName).Result == null)
            {
                var user = new ApplicationUser();
                user.UserName = this.adminUserName;
                user.Email = this.adminUserName;

                var result = await userManager.CreateAsync(user, "Px/Y@+TPPYggfVO!g;8U");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, administratorRoleName);
                }
            }
        }
    }
}
