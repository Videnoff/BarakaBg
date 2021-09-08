namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
