namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class CreateProductInputModel : BaseProductInputModel, IMapTo<Product>
    {
        [Display(Name = "Add Images")]
        public IEnumerable<IFormFile> UploadedImages { get; set; }

        //public IEnumerable<ProductIngredientInputModel> Ingredients { get; set; }
    }
}
