using BarakaBg.Data.Models.Credentials;
using Microsoft.Extensions.Options;

namespace BarakaBg.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class AdminSeeder : ISeeder
    {
        private readonly AdminCredentials adminCredentials;

        public AdminSeeder(AdminCredentials adminCredentials)
        {
            this.adminCredentials = adminCredentials;
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await this.SeedRoleAsync(userManager, this.adminCredentials);
        }

        private async Task SeedRoleAsync(UserManager<ApplicationUser> userManager, AdminCredentials adminCredentials)
        {
            if (userManager.FindByNameAsync(this.adminCredentials.Username).Result == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = this.adminCredentials.Username,
                    Email = this.adminCredentials.Username,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(admin, this.adminCredentials.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, GlobalConstants.AdministratorName);
                }
            }
        }
    }
}
