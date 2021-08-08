namespace BarakaBg.Web.ViewModels.Administration.Products
{
    using System.Globalization;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;

    public class DeletedProductViewModel : SingleProductViewModel, IHaveCustomMappings
    {
        public string DeletedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, DeletedProductViewModel>()
                .ForMember(
                    source => source.ImageUrl,
                    destination => destination.MapFrom(member => member.Images.FirstOrDefault().RemoteImageUrl))
                .ForMember(
                    source => source.DeletedOn,
                    destination => destination.MapFrom(member => member.DeletedOn.Value.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)));
        }
    }
}