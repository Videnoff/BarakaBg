using System.Security.Claims;

namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Web.ViewModels.Administration.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class UsersController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IDeletableEntityRepository<ApplicationUser> usersRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                this.ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found!";
                return this.NotFound();
            }

            var existingUserClaims = await this.userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId,
            };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                var userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };

                /*
                 * If the user has the claim, ser IsSelected property to true, so the checkbox next to the claim is checked on the UI
                 */
                if (existingUserClaims.Any(x => x.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                this.ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found!";
                return this.NotFound();
            }

            var claims = await this.userManager.GetClaimsAsync(user);
            var result = await this.userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError(string.Empty, "Cannot remove user existing claims");
                return this.View();
            }

            result = await this.userManager.AddClaimsAsync(user, model.Claims
                .Where(x => x.IsSelected)
                .Select(c => new Claim(c.ClaimType, c.ClaimType)));

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError(string.Empty, "Cannot add selected claims to user!");
                return this.View();
            }

            return this.RedirectToAction("EditUser", new { Id = model.UserId });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = this.userManager.Users;
            return this.View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                this.ViewBag.ErrorMessage = $"User with Id = {id} cannot be found!";
                return this.NotFound();
            }

            var userClaims = await this.userManager.GetClaimsAsync(user);
            var userRoles = await this.userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                City = user.Town,
                Claims = userClaims.Select(x => x.Value).ToList(),
                Roles = userRoles,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await this.userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                this.ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found!";
                return this.NotFound();
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.Username;
                user.Town = model.City;

                var result = await this.userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                this.ViewBag.ErrorMessage = $"User with Id = {id} cannot be found!";
                return this.NotFound();
            }
            else
            {
                var result = await this.userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                //await this.usersRepository
                //    .All()
                //    .Include(p => p.ShoppingBag)
                //    .Include(p => p.WishListProducts)
                //    .FirstOrDefaultAsync(m => m.Id == id);

                return this.View("ListUsers");
            }
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = this.roleManager.Roles;
            return this.View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var applicationRole = new ApplicationRole
                {
                    Name = model.RoleName,
                };

                var result = await this.roleManager.CreateAsync(applicationRole);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("ListRoles", "Users", new { area = "Administration" });
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await this.roleManager.FindByIdAsync(id);

            if (role == null)
            {
                this.ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found!";
                return this.NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };

            foreach (var user in this.userManager.Users)
            {
                if (await this.userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await this.roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                this.ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found!";
                return this.NotFound();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await this.roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            this.ViewBag.roleId = roleId;

            var role = await this.roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                this.ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found!";
                return this.NotFound();
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in this.userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                };

                if (await this.userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await this.roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                this.ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return this.NotFound();
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await this.userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !await this.userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await this.userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await this.userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await this.userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return this.RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return this.RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await this.roleManager.FindByIdAsync(id);

            if (role == null)
            {
                this.ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found!";
                return this.NotFound();
            }
            else
            {
                var result = await this.roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View("ListRoles");
            }
        }
    }
}
