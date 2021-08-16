namespace BarakaBg.Web.ViewModels.SearchProducts
{
    using System.Collections.Generic;

    using BarakaBg.Web.ViewModels.Home;
    using BarakaBg.Web.ViewModels.Products;

    public class AllProductsQueryModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<ProductInListViewModel> Products { get; set; }

        public IEnumerable<IndexPageProductViewModel> RandomProducts { get; set; }

        public int ProductsCount { get; set; }

        public int CategoriesCount { get; set; }

        public int IngredientsCount { get; set; }

        public int ImagesCount { get; set; }
    }
}
