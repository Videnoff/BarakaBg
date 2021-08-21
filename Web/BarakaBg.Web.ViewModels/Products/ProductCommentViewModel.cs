namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.Globalization;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class ProductCommentViewModel : IMapFrom<ProductComment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public byte Rating { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }

        public string UserEmail { get; set; }

        public string Title { get; set; }

        public List<ProductComment> ListOfComments { get; set; }

        public string Comment { get; set; }

        public int ProductId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProductComment, ProductCommentViewModel>()
                .ForMember(
                    source => source.CreatedOn,
                    destination => destination.MapFrom(member => member.CreatedOn.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)));
        }
    }
}
