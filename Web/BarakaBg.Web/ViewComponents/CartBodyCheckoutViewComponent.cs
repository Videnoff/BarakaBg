using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BarakaBg.Services;
using BarakaBg.Services.Data;
using BarakaBg.Web.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace BarakaBg.Web.ViewComponents
{
    public class CartBodyCheckoutViewComponent : ViewComponent
    {
        private readonly IShoppingBagService shoppingBagService;
        private readonly ITextService textService;

        public CartBodyCheckoutViewComponent(IShoppingBagService shoppingBagService, ITextService textService)
        {
            this.shoppingBagService = shoppingBagService;
            this.textService = textService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await this.shoppingBagService.GetAllProductsAsync<ShoppingBagProductViewModel>(this.User.Identity.IsAuthenticated, this.HttpContext.Session, userId);

            if (products != null)
            {
                foreach (var product in products)
                {
                    product.ProductName = this.textService.TruncateAtWord(product.ProductName, 30);
                }
            }

            var viewModel = new ShoppingBagViewModel
            {
                Products = products ?? new List<ShoppingBagProductViewModel>(),
            };

            return this.View(viewModel);
        }
    }
}
