namespace BarakaBg.Web.ViewModels.Addresses
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Models;

    public class AddressInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Street { get; set; }

        public string Description { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string City { get; set; }

        public string ZipCode { get; set; }

        public string UserId { get; set; }
    }
}
