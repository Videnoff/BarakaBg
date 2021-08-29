namespace BarakaBg.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.Claims = new List<string>();
            this.Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}
