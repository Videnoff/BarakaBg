namespace BarakaBg.Web.ViewModels.Products
{
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class IngredientsViewModel : IMapFrom<ProductIngredient>
    {
        public string IngredientName { get; set; }
    }
}
