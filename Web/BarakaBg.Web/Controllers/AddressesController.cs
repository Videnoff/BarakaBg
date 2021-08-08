namespace BarakaBg.Web.Controllers
{
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Addresses;
    using Microsoft.AspNetCore.Mvc;

    public class AddressesController : BaseController
    {
        private readonly IAddressesService addressesService;

        public AddressesController(IAddressesService addressesService)
        {
            this.addressesService = addressesService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddressInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Create", "Orders");
            }

            var createResult = await this.addressesService.CreateAsync(model);

            if (createResult)
            {
                this.TempData["Alert"] = "Successfully added address.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem adding the address.";
            }

            return this.RedirectToAction("Create", "Orders");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var deleteResult = await this.addressesService.DeleteAsync(id);

            if (deleteResult)
            {
                this.TempData["Alert"] = "Successfully deleted address.";
            }
            else
            {
                this.TempData["Error"] = "There was a problem deleting the address.";
            }

            return this.RedirectToAction("Create", "Orders");
        }
    }
}
