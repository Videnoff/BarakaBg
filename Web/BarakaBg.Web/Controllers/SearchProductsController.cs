namespace BarakaBg.Web.Controllers
{
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.SearchProducts;
    using Microsoft.AspNetCore.Mvc;

    public class SearchProductsController : BaseController
    {
        private readonly IProductsService productsService;

        public SearchProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public IActionResult Index()
        {
            // TODO: List of all ingredients
            var viewModel = new SearchIndexViewModel
            {

            };
            return this.View();
        }

        [HttpGet]
        public IActionResult List(SearchListInputModel input)
        {
            var viewModel = new ListViewModel
            {
                // TODO: Products = 
            };
            return this.View();
        }
    }
}