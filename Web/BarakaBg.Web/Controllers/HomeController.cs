namespace BarakaBg.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;

    using Azure.Storage.Blobs;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels;
    using BarakaBg.Web.ViewModels.Home;
    using BarakaBg.Web.ViewModels.Products;
    using BarakaBg.Web.ViewModels.SearchProducts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class HomeController : BaseController
    {
        private readonly IGetCountsService countsService;
        private readonly IProductsService productsService;
        private readonly IRepository<Product> productRepository;

        public HomeController(
            IGetCountsService countsService,
            IProductsService productsService,
            IConfiguration configuration,
            IRepository<Product> productRepository)
        {
            this.countsService = countsService;
            this.productsService = productsService;
            this.productRepository = productRepository;
        }

        public IActionResult Index(string searchTerm)
        {
            var countsDto = this.countsService.GetCounts();

            //// var viewModel = this.mapper.Map<IndexViewModel>(countsDto);
            //var viewModel = new IndexViewModel
            //{
            //    CategoriesCount = countsDto.CategoriesCount,
            //    ImagesCount = countsDto.ImagesCount,
            //    IngredientsCount = countsDto.IngredientsCount,
            //    ProductsCount = countsDto.ProductsCount,
            //    RandomProducts = this.productsService.GetRandom<IndexPageProductViewModel>(10),
            //};

            //return this.View(viewModel);

            var productsQuery = this.productRepository.All().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(x =>
                    x.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    x.Category.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    x.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            var products = productsQuery
                .OrderByDescending(x => x.Id)
                .Select(x =>
                    new ProductInListViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        Description = x.Description,
                        ImageUrl = x.OriginalUrl,
                        Price = x.Price,
                        Stock = x.Stock,
                    })
                .ToList();

            return this.View(new AllProductsQueryModel
            {
                Products = products,
                SearchTerm = searchTerm,
                RandomProducts = this.productsService.GetRandom<IndexPageProductViewModel>(10),
                CategoriesCount = countsDto.CategoriesCount,
                ImagesCount = countsDto.ImagesCount,
                IngredientsCount = countsDto.IngredientsCount,
                ProductsCount = countsDto.ProductsCount,
            });
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
