namespace BarakaBg.Web.Controllers
{
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class IdentityTestController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationUser> roleManager;

        public IdentityTestController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationUser> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Create()
        {
            var user = new ApplicationUser()
            {
                Email = "",
                UserName = "",
                EmailConfirmed = true,
                PhoneNumber = "9876543210",
            };

            var result = await this.userManager.CreateAsync(user, "123456");
            return this.Ok("User created");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> WhoAmI()
        {
            if (!await this.roleManager.RoleExistsAsync("Admin"))
            {
                await this.roleManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "Admin",
                });
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var result = await this.userManager.AddToRoleAsync(user, "Admin");
            return this.Json(result);
        }
    }
}