namespace BarakaBg.Web.ViewModels.SearchProducts
{
    using System.Collections.Generic;

    using BarakaBg.Web.ViewModels.Products;

    public class ListViewModel
    {
        public IEnumerable<ProductInListViewModel> Products { get; set; }
    }
}