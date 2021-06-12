namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;

    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class EditProductInputModel : BaseProductInputModel, IMapFrom<Product>
    {
        public int Id { get; set; }
    }
}