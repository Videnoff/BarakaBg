namespace BarakaBg.Web.ViewModels.Home
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<IndexPageProductViewModel> RandomProducts { get; set; }

        public string SearchTerm { get; set; }

        public int ProductsCount { get; set; }

        public int CategoriesCount { get; set; }

        public int IngredientsCount { get; set; }

        public int ImagesCount { get; set; }
    }
}
