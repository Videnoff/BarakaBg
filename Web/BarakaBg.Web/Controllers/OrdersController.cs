namespace BarakaBg.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Addresses;
    using BarakaBg.Web.ViewModels.Orders;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class OrdersController : BaseController
    {
        private const string ShoppingBagMessage = "The shopping cart is empty";

        private readonly IOrdersService ordersService;
        private readonly IAddressesService addressesService;
        private readonly ICountriesService countriesService;
        private readonly ITextService textService;
        private readonly IShoppingBagService shoppingBagService;

        private readonly string userId;

        public OrdersController(
            IShoppingBagService shoppingBagService,
            IHttpContextAccessor contextAccessor,
            IOrdersService ordersService,
            IAddressesService addressesService,
            ICountriesService countriesService)
        {
            this.shoppingBagService = shoppingBagService;
            this.ordersService = ordersService;
            this.addressesService = addressesService;
            this.countriesService = countriesService;

            this.userId = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IActionResult> Create()
        {
            var hasProduct = await this.shoppingBagService.AnyProductsAsync(this.userId);

            if (!hasProduct)
            {
                this.TempData["Error"] = ShoppingBagMessage;
                return this.RedirectToAction("Index", "Home");
            }

            var addresses = this.addressesService.GetAll<AddressViewModel>(this.userId);

            foreach (var address in addresses)
            {
                if (!string.IsNullOrEmpty(address.Description))
                {
                    address.Description = this.textService.TruncateAtWord(address.Description, 30);
                }
            }

            var countries = this.countriesService.GetAll();

            var email = this.User.Identity.Name;

            var model = new OrderCreateInputModel
            {
                Addresses = addresses,
                Email = email,
                Countries = countries,
            };

            await this.ordersService.CancelAnyProcessingOrders(this.userId);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                var hasProducts = await this.shoppingBagService.AnyProductsAsync(this.userId);

                if (!hasProducts)
                {
                    this.TempData["Error"] = ShoppingBagMessage;
                    return this.RedirectToAction("Index", "Home");
                }

                var addresses = this.addressesService.GetAll<AddressViewModel>(this.userId);

                foreach (var address in addresses)
                {
                    address.Description = this.textService.TruncateAtWord(address.Description, 30);
                }

                var countries = this.countriesService.GetAll();

                var email = this.User.Identity.Name;

                input.Addresses = addresses;
                input.Email = email;
                input.Countries = countries;

                return this.View(input);
            }

            await this.ordersService.CreateAsync<OrderCreateInputModel>(input, this.userId);

            return this.RedirectToAction(nameof(this.Complete));
        }

        public async Task<IActionResult> Complete()
        {
            var hasProducts = await this.shoppingBagService.AnyProductsAsync(this.userId);
            if (!hasProducts)
            {
                this.TempData["Error"] = ShoppingBagMessage;
                return this.RedirectToAction("Index", "Home");
            }

            var orderId = await this.ordersService.CompleteOrderAsync(this.userId);

            if (this.ordersService.GetPayFormById(orderId) == PayForm.CashOnDelivery)
            {
                this.TempData["Alert"] = "Successfully registered order.";
            }

            var order = this.ordersService.GetById<OrderPayConditionViewModel>(orderId);

            return this.View(order);
        }

        [HttpGet("/Orders/History/{pageNumber?}")]
        public IActionResult History(int pageNumber = 1)
        {
            if (pageNumber <= 0)
            {
                return this.History();
            }

            var itemsPerPage = 6;
            var orders = this.ordersService.TakeOrdersByUserId<OrderCheckViewModel>(this.userId, pageNumber, itemsPerPage);
            var ordersCount = this.ordersService.GetOrdersCountByUserId(this.userId);

            var viewModel = new OrderListViewModel
            {
                ItemsCount = ordersCount,
                ItemsPerPage = itemsPerPage,
                PageNumber = pageNumber,
                Orders = orders,
            };

            return this.View(viewModel);
        }

        public IActionResult Details(string id)
        {
            if (!this.ordersService.UserHasOrder(this.userId, id))
            {
                this.TempData["Error"] = "Order not found.";
                return this.RedirectToAction("Index", "Home");
            }

            this.ViewData["OrderId"] = id;
            return this.View();
        }
    }
}
