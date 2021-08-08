namespace BarakaBg.Web.ViewModels.Orders
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Addresses;

    public class OrderCreateInputModel : IMapTo<Order>
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]

        [Display(Name = "Full Name")]
        public string UserFullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[+]?[0-9]+$")]
        public string Phone { get; set; }

        [Required]
        public string AddressId { get; set; }

        public IEnumerable<AddressViewModel> Addresses { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        public PayForm PayForm { get; set; }
    }
}
