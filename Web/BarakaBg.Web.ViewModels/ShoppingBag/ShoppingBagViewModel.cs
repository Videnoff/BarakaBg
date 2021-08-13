namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.Linq;

    public class ShoppingBagViewModel
    {
        public IEnumerable<ShoppingBagProductViewModel> Products { get; set; }

        public decimal GrandTotalPrice => this.Products.Sum(x => x.TotalPrice);
    }
}
