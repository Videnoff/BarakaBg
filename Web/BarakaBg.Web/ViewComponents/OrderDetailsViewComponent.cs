namespace BarakaBg.Web.ViewComponents
{
    using BarakaBg.Services;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Orders;
    using Microsoft.AspNetCore.Mvc;

    public class OrderDetailsViewComponent : ViewComponent
    {
        private readonly IOrdersService ordersService;
        private readonly ITextService textService;

        public OrderDetailsViewComponent(
            IOrdersService ordersService,
            ITextService textService)
        {
            this.ordersService = ordersService;
            this.textService = textService;
        }

        public IViewComponentResult Invoke(string orderId)
        {
            var order = this.ordersService.GetById<OrderViewModel>(orderId);

            foreach (var product in order.Products)
            {
                product.ProductName = this.textService.TruncateAtWord(product.ProductName, 30);
            }

            return this.View(order);
        }
    }
}
