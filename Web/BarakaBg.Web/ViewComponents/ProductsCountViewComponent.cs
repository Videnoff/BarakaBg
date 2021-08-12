namespace BarakaBg.Web.ViewComponents
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.WishListAndShoppingBag;
    using Microsoft.AspNetCore.Mvc;

    public class ProductsCountViewComponent : ViewComponent
    {
        private readonly IWishListService wishListService;
        private readonly IShoppingBagService shoppingBagService;

        public ProductsCountViewComponent(
            IWishListService wishListService,
            IShoppingBagService shoppingBagService)
        {
            this.wishListService = wishListService;
            this.shoppingBagService = shoppingBagService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishListItemsCount = this.wishListService.GetCount(userId);
            var shoppingBagItemsCount = await this.shoppingBagService.GetProductsCountAsync(this.User.Identity.IsAuthenticated, this.HttpContext.Session, userId);

            var viewModel = new WishListAndShoppingBagViewModel
            {
                WishListProductsCount = wishListItemsCount,
                ShoppingBagProductsCount = shoppingBagItemsCount,
            };

            return this.View(viewModel);
        }
    }
}
