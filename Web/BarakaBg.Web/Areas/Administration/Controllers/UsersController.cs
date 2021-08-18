namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BarakaBg.Data.Models;
    using BarakaBg.Web.ViewModels.Administration.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("RegisteredUsers")]
        public IActionResult RegisteredUsers()
        {
            var registeredUsers = new List<RegisteredUsersViewModel>();
            var userDate = this.userManager
                .Users
                .OrderBy(x => x.CreatedOn)
                .ToList()
                .GroupBy(x => x.CreatedOn.ToString("dd-MMM-yyy", CultureInfo.InvariantCulture));

            var usersCount = 0;

            foreach (var user in userDate)
            {
                usersCount += user.Count();
                registeredUsers.Add(new RegisteredUsersViewModel
                {
                    RegistrationDate = user.Key,
                    UsersCount = usersCount,
                });
            }

            return this.Json(registeredUsers);
        }
    }
}
