namespace BarakaBg.Services.Models
{
    using System.Collections.Generic;

    public class ProductDto
    {
        public ProductDto()
        {
            this.Ingredients = new List<string>();
        }

        public string ProductName { get; set; }

        public string CategoryName { get; set; }

        public string Brand { get; set; }

        public string LinkToBrand { get; set; }

        public string ProductCode { get; set; }

        public string Stock { get; set; }

        public string ProductDescription { get; set; }

        public string Content { get; set; }

        public List<string> Ingredients { get; set; }

        public decimal Price { get; set; }

        public string OriginalUrl { get; set; }

        public string ImageUrl { get; set; }
    }
}
