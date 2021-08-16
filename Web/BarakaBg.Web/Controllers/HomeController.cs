namespace BarakaBg.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels;
    using BarakaBg.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IGetCountsService countsService;
        private readonly IProductsService productsService;
        private readonly IRepository<Product> productRepository;

        public HomeController(
            IGetCountsService countsService,
            IProductsService productsService,
            IRepository<Product> productRepository)
        {
            this.countsService = countsService;
            this.productsService = productsService;
            this.productRepository = productRepository;
        }

        public IActionResult Index(string searchTerm)
        {
            var countsDto = this.countsService.GetCounts();

            // var viewModel = this.mapper.Map<IndexViewModel>(countsDto);
            var viewModel = new IndexViewModel
            {
                CategoriesCount = countsDto.CategoriesCount,
                ImagesCount = countsDto.ImagesCount,
                IngredientsCount = countsDto.IngredientsCount,
                ProductsCount = countsDto.ProductsCount,
                RandomProducts = this.productsService.GetRandom<IndexPageProductViewModel>(10),
            };

            if (string.IsNullOrEmpty(searchTerm))
            {
                return this.View(viewModel);
            }

            viewModel = new IndexViewModel
            {
                CategoriesCount = countsDto.CategoriesCount,
                ImagesCount = countsDto.ImagesCount,
                IngredientsCount = countsDto.IngredientsCount,
                ProductsCount = countsDto.ProductsCount,
                RandomProducts = this.productsService.GetRandom<IndexPageProductViewModel>(10).Where(x =>
                        x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                        x.CategoryName.ToLower().Contains(searchTerm.ToLower())),
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return this.View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            });
        }
    }
}
