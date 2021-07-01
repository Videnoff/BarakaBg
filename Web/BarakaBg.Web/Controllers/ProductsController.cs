namespace BarakaBg.Web.Controllers
{
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ProductsController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IProductsService productsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public ProductsController(
            ICategoriesService categoriesService,
            IProductsService productsService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            this.categoriesService = categoriesService;
            this.productsService = productsService;
            this.userManager = userManager;
            this.environment = environment;
        }

        public IActionResult All(int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 12;
            var viewModel = new ProductsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                ProductsCount = this.productsService.GetCount(),
                Products = this.productsService.GetAll<ProductInListViewModel>(id, ItemsPerPage),
            };

            return this.View(viewModel);
        }

        public IActionResult ById(int id)
        {
            var product = this.productsService.GetById<SingleProductViewModel>(id);
            return this.View(product);
        }
    }
}
