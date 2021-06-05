namespace BarakaBg.Web.ViewModels.Products
{
    using Microsoft.AspNetCore.Http;

    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateProductInputModel
    {
        [Required]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string Brand { get; set; }

        [MinLength(3)]
        public string ProductCode { get; set; }

        [Required]
        public string Stock { get; set; }

        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [MinLength(100)]
        public string Description { get; set; }

        [Required]
        public string Content { get; set; }

        public string Feedback { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<IFormFile> Images { get; set; }

        public IEnumerable<ProductIngredientInputModel> Ingredients { get; set; }

        public IEnumerable<KeyValuePair<string, string>> CategoriesItems { get; set; }
    }
}
