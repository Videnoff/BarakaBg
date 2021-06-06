namespace BarakaBg.Web.ViewModels.Home
{
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;

    public class IndexPageProductViewModel : IMapFrom<Product>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, IndexPageProductViewModel>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/products/" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension));
        }
    }
}