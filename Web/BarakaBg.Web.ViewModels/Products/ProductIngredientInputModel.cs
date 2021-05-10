namespace BarakaBg.Web.ViewModels.Products
{
    using System.ComponentModel.DataAnnotations;

    public class ProductIngredientInputModel
    {
        [Required]
        [MinLength(3)]
        public string IngredientName { get; set; }
    }
}