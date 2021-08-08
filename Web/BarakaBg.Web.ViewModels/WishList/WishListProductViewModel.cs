namespace BarakaBg.Web.ViewModels.Favorites
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
                    source => source.ImageUrl,
                    destination => destination.MapFrom(member => member.Product.Images.FirstOrDefault().RemoteImageUrl))
                .ForMember(
                    source => source.AverageRating,
                    destination => destination.MapFrom(member => (!member.Product.Reviews.Any()) ? 0 : Math.Round(member.Product.Reviews.Average(x => x.Rating), 2)));
        }
    }
}
