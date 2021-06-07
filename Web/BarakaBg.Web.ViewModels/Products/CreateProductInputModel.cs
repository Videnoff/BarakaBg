namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class CreateProductInputModel : BaseProductInputModel
    {
        public IEnumerable<IFormFile> Images { get; set; }

        public IEnumerable<ProductIngredientInputModel> Ingredients { get; set; }
    }
}
