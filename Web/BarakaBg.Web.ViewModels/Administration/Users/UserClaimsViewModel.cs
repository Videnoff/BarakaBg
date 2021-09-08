namespace BarakaBg.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    using BarakaBg.Data.Models.Claims;

    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            this.UserClaims = new List<UserClaim>();
        }

        public string UserId { get; set; }

        public List<UserClaim> UserClaims { get; set; }
    }
}
