namespace BarakaBg.Web.Controllers
{
    using System.Threading.Tasks;

    using BarakaBg.Services;
    using Microsoft.AspNetCore.Mvc;

    // TODO: Move in Administration area
    public class GatherProductsController : BaseController
    {
        private readonly IBarakaBgScraperService barakaBgScraperService;

        public GatherProductsController(IBarakaBgScraperService barakaBgScraperService)
        {
            this.barakaBgScraperService = barakaBgScraperService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Add()
        {
            await this.barakaBgScraperService.PopulateDbWithProductsAsync(2000);

            return this.RedirectToAction("Index");
        }
    }
}
