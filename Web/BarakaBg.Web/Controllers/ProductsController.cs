namespace BarakaBg.Web.Controllers
{
    using System;
    using System.Linq;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Home;
    using BarakaBg.Web.ViewModels.Products;
    using BarakaBg.Web.ViewModels.SearchProducts;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ProductsController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IProductsService productsService;
        private readonly ITextService textService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;
        private readonly IRepository<Product> productRepository;

        public ProductsController(
            ICategoriesService categoriesService,
            IProductsService productsService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            ITextService textService,
            IRepository<Product> productRepository)
        {
            this.categoriesService = categoriesService;
            this.productsService = productsService;
            this.userManager = userManager;
            this.environment = environment;
            this.textService = textService;
            this.productRepository = productRepository;
        }

        public IActionResult All(string searchTerm, int id = 1)
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
                ItemsCount = this.productsService.GetCount(),
                Products = this.productsService.GetAll<ProductInListViewModel>(id),
            };

            if (string.IsNullOrEmpty(searchTerm))
            {
                return this.View(viewModel);
            }

            viewModel = new ProductsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                ItemsCount = this.productsService.GetCount(),
                Products = this.productsService.GetAll<ProductInListViewModel>(id).Where(x =>
                    x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    x.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                    x.Description.ToLower().Contains(searchTerm.ToLower())),
            };

            return this.View(viewModel);
        }

        public IActionResult ById(int id)
        {
            var product = this.productsService.GetById<SingleProductViewModel>(id);
            product.Description = this.textService.TruncateAtWord(product.Description, 200);
            return this.View(product);
        }
    }
}
