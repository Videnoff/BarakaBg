﻿namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class SingleProductViewModel : IMapFrom<Product>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Stock { get; set; }

        public double AverageVote { get; set; }

        public IEnumerable<IngredientsViewModel> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, SingleProductViewModel>()
                .ForMember(
                    x => x.AverageVote,
                    opt => opt.MapFrom(x => x.Votes.Count == 0 ? 0 : x.Votes.Average(v => v.Value)))
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/products/" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension));
        }
    }
}