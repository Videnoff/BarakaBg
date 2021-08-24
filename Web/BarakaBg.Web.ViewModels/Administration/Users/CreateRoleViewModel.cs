namespace BarakaBg.Web.ViewModels.Administration.Users
{
    using System.ComponentModel.DataAnnotations;

    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
