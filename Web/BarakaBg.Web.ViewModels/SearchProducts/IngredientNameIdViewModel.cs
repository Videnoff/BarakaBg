namespace BarakaBg.Web.ViewModels.SearchProducts
{
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class IngredientNameIdViewModel : IMapFrom<Ingredient>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}