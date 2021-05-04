namespace BarakaBg.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;

    using BarakaBg.Data;
    using BarakaBg.Web.ViewModels;
    using BarakaBg.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                CategoriesCount = this.dbContext.Categories.Count(),
                ImagesCount = this.dbContext.Images.Count(),
                IngredientsCount = this.dbContext.Ingredients.Count(),
                ProductsCount = this.dbContext.Products.Count(),
            };

            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
