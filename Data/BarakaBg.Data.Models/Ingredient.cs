namespace BarakaBg.Data.Models
{
    using System.Collections.Generic;

    using BarakaBg.Data.Common.Models;

    public class Ingredient : BaseDeletableModel<int>
    {
        public Ingredient()
        {
            this.Products = new HashSet<ProductIngredient>();
        }

        public string Name { get; set; }

        public ICollection<ProductIngredient> Products { get; set; }
    }
}