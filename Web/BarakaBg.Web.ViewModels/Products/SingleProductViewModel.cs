namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class SingleProductViewModel : IMapFrom<Product>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public IEnumerable<IngredientsViewModel> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, ProductInListViewModel>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/products/" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension));
        }
    }
}