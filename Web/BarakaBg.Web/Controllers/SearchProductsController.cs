namespace BarakaBg.Web.Controllers
{
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Products;
    using BarakaBg.Web.ViewModels.SearchProducts;
    using Microsoft.AspNetCore.Mvc;

    public class SearchProductsController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly IIngredientsService ingredientsService;

        public SearchProductsController(
            IProductsService productsService,
            IIngredientsService ingredientsService)
        {
            this.productsService = productsService;
            this.ingredientsService = ingredientsService;
        }

        public IActionResult Index()
        {
            // TODO: List of all ingredients
            var viewModel = new SearchIndexViewModel
            {
                Ingredients = this.ingredientsService.GetAllPopular<IngredientNameIdViewModel>(),
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public IActionResult List(SearchListInputModel input)
        {
            var viewModel = new ListViewModel
            {
                Products = this.productsService
                    .GetByIngredients<ProductInListViewModel>(input.Ingredients),
            };

            return this.View(viewModel);
        }
    }
}