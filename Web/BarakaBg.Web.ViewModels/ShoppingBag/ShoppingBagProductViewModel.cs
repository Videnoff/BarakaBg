namespace BarakaBg.Web.ViewModels.Products
{
    using System;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class ShoppingBagProductViewModel : IMapFrom<ShoppingBagProduct>, IHaveCustomMappings
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        public double AverageRating { get; set; }

        [IgnoreMap]
        public decimal TotalPrice => this.Quantity * this.ProductPrice;

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ShoppingBagProduct, ShoppingBagProductViewModel>()
                .ForMember(
                    x => x.AverageRating,
                    opt => opt.MapFrom(member => (!member.Product.Reviews.Any()) ? 0 : Math.Round(member.Product.Reviews.Average(x => x.Rating), 2)))
                .ForMember(
                    x => x.ImageUrl, opt => opt.MapFrom(x => x.Product.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Product.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/products/" + x.Product.Images.FirstOrDefault().Id + "." + x.Product.Images.FirstOrDefault().Extension));
        }
    }
}
