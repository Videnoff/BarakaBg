namespace BarakaBg.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.WishList;
    using Microsoft.AspNetCore.Mvc;

    public class WishListController : BaseController
    {
        private readonly IWishListService wishListService;

        public WishListController(IWishListService wishListService)
        {
            this.wishListService = wishListService;
        }

        public IActionResult All()
        {
            var wishListProducts = this.wishListService
                .GetAll<WishListProductViewModel>(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return this.View(wishListProducts);
        }

        public async Task<IActionResult> Add(int id)
        {
            var addResult = await this.wishListService.AddAsync(id, this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (addResult)
            {
                this.TempData["Alert"] = "Successfully added product to wishList.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem adding the product to wishList.";
            }

            return this.RedirectToAction(nameof(this.All));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await this.wishListService.DeleteAsync(id, this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (deleteResult)
            {
                this.TempData["Alert"] = "Successfully removed product from wishList.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem removing the product from wishList.";
            }

            return this.RedirectToAction(nameof(this.All));
        }
    }
}
