namespace BarakaBg.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ShoppingBagController : BaseController
    {
        private readonly IShoppingBagService shoppingBagService;
        private readonly IProductsService productsService;

        private readonly string userId;
        private readonly bool isUserAuthenticated;
        private readonly ISession session;

        public ShoppingBagController(
            IShoppingBagService shoppingBagService,
            IProductsService productsService,
            IHttpContextAccessor contextAccessor)
        {
            this.shoppingBagService = shoppingBagService;
            this.productsService = productsService;

            this.userId = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            this.isUserAuthenticated = contextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.session = contextAccessor.HttpContext.Session;
        }

        public async Task<IActionResult> Index()
        {
            var shoppingBagProducts =
                await this.shoppingBagService
                    .GetAllProductsAsync<ShoppingBagProductViewModel>(
                        this.isUserAuthenticated,
                        this.session,
                        this.userId);

            if (shoppingBagProducts == null || !shoppingBagProducts.Any())
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View(new ShoppingBagViewModel()
            {
                Products = shoppingBagProducts,
            });
        }

        [HttpGet("/ShoppingBag/Add/{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            var addResult =
                await this.shoppingBagService.AddProductAsync(this.isUserAuthenticated, this.session, this.userId, productId);

            if (addResult)
            {
                this.TempData["Alert"] = "Successfully added the product to the shopping cart.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem adding the product to the shopping cart.";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet("/ShoppingBag/Delete/{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var deleteResult = await this.shoppingBagService.DeleteProductAsync(this.isUserAuthenticated, this.session, this.userId, productId);

            if (deleteResult)
            {
                this.TempData["Alert"] = "Successfully removed the product from the shopping cart.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem removing the product from the shopping cart.";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpGet("/ShoppingBag/Quantity/{productId}")]
        public async Task<IActionResult> UpdateQuantity(int productId, bool increase)
        {
            var updateResult = await this.shoppingBagService.UpdateQuantityAsync(this.isUserAuthenticated, this.session, this.userId, productId, increase);

            if (updateResult)
            {
                this.TempData["Alert"] = "Successfully updated product quantity.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem updating the product quantity.";
            }

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
