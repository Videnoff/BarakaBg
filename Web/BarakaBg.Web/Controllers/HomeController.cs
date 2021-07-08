    namespace BarakaBg.Web.Controllers
{
    using System.Diagnostics;

    using Azure.Storage.Blobs;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels;
    using BarakaBg.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class HomeController : BaseController
    {
        private readonly IGetCountsService countsService;
        private readonly IProductsService productsService;

        public HomeController(
            IGetCountsService countsService,
            IProductsService productsService,
            IConfiguration configuration)
        {
            this.countsService = countsService;
            this.productsService = productsService;
        }

        public IActionResult Index()
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
