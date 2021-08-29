namespace BarakaBg.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    using BarakaBg.Data.Models;

    public class UserClaimsViewModel
    {
        public string UserId { get; set; }

        public List<UserClaim> Claims { get; set; }
    }
}
