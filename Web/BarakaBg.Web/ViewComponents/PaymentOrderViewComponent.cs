namespace BarakaBg.Web.ViewComponents
{
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Payment;
    using Microsoft.AspNetCore.Mvc;

    public class PaymentOrderViewComponent : ViewComponent
    {
        private readonly IOrdersService ordersService;

        public PaymentOrderViewComponent(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public IViewComponentResult Invoke(string orderId)
        {
            var payForm = this.ordersService.GetPayFormById(orderId);

            var viewModel = new PaymentViewModel
            {
                PayForm = payForm,
            };

            return this.View(viewModel);
        }
    }
}
