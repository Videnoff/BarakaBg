namespace BarakaBg.Web.ViewModels.SearchProducts
{
    using System.Collections.Generic;

    public class SearchIndexViewModel
    {
        public IEnumerable<IngredientNameIdViewModel> Ingredient { get; set; }
    }
}