namespace BarakaBg.Data.Models
{
    using System.Collections.Generic;

    using BarakaBg.Data.Common.Models;

    public class Category : BaseDeletableModel<int>
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }

        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
