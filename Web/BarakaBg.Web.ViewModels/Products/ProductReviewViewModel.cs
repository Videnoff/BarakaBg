using System.Globalization;
using AutoMapper;
using BarakaBg.Common;
using BarakaBg.Data.Models;
using BarakaBg.Services.Mapping;

namespace BarakaBg.Web.ViewModels.Products
{
    public class ProductReviewViewModel : IMapTo<UserProductReview>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public byte Rating { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }

        public string UserEmail { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UserProductReview, ProductReviewViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    opt => opt.MapFrom(member => member.CreatedOn.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)));
        }
    }
}
