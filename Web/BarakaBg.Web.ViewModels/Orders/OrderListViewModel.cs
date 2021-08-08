namespace BarakaBg.Web.ViewModels.Orders
{
    using System.Collections.Generic;

    public class OrderListViewModel : PagingViewModel
    {
        public IEnumerable<OrderCheckViewModel> Orders { get; set; }
    }
}
