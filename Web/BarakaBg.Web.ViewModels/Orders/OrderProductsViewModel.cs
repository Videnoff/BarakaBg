namespace BarakaBg.Web.ViewModels.Orders
{
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class OrderProductsViewModel : IMapFrom<ProductOrder>, IHaveCustomMappings
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProductOrder, OrderProductsViewModel>()
                .ForMember(
                    source => source.ImageUrl,
                    destination => destination.MapFrom(member => (!member.Product.Images.Any()) ? GlobalConstants.ImageNotFoundPath : member.Product.Images.FirstOrDefault().RemoteImageUrl));
        }
    }
}
