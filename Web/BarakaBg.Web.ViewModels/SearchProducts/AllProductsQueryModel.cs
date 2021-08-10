namespace BarakaBg.Web.ViewModels.SearchProducts
{
    using System.Collections.Generic;

    using BarakaBg.Web.ViewModels.Products;

    public class AllProductsQueryModel : PagingViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<ProductInListViewModel> Products { get; set; }
    }
}