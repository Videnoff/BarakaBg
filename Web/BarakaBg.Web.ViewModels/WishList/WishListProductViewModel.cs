namespace BarakaBg.Web.ViewModels.WishList
{
    using System;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class WishListProductViewModel : IMapFrom<WishList>, IHaveCustomMappings
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public string ImageUrl { get; set; }

        public double AverageRating { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<WishList, WishListProductViewModel>()
                .ForMember(
                    x => x.ImageUrl, opt => opt.MapFrom(x => x.Product.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Product.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/products/" + x.Product.Images.FirstOrDefault().Id + "." + x.Product.Images.FirstOrDefault().Extension))
                .ForMember(
                    x => x.AverageRating,
                    opt => opt.MapFrom(member => (!member.Product.Reviews.Any()) ? 0 : Math.Round(member.Product.Reviews.Average(x => x.Rating), 2)));
        }
    }
}
