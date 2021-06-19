namespace BarakaBg.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public Country()
        {
            this.Cities = new HashSet<City>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}