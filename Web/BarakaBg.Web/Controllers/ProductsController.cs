namespace BarakaBg.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    //[Authorize(Roles = "Administrator")]
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

        public IActionResult Create()
        {
            var viewModel = new CreateProductInputModel();
            viewModel.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
                return this.View(input);
            }

            // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await this.userManager.GetUserAsync(this.User);
            try
            {
                await this.productsService.CreateAsync(input, user.Id, $"{this.environment.WebRootPath}/images");
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
                input.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
                return this.View(input);
            }

            // return this.Json(input);
            return this.Redirect("/");
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
    }
}
