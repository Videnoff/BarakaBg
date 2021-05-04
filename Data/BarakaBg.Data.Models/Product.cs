namespace BarakaBg.Data.Models
{
    using System.Collections.Generic;

    using BarakaBg.Data.Common.Models;

    public class Product : BaseDeletableModel<int>
    {
        public Product()
        {
            this.Ingredients = new HashSet<ProductIngredient>();
            this.Images = new HashSet<Image>();
        }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string ProductCode { get; set; }

        public string Stock { get; set; }

        public double Price { get; set; }



        public string Description { get; set; }

        public string Content { get; set; }

        public string Feedback { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ProductIngredient> Ingredients { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}