using BarakaBg.Data.Models;
using BarakaBg.Services.Mapping;

namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;

    public class EditProductInputModel : BaseProductInputModel, IMapFrom<Product>
    {
        public int Id { get; set; }
    }
}