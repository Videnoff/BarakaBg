namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using BarakaBg.Common;
    using BarakaBg.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
