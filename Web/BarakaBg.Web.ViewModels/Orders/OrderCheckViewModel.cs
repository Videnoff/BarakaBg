namespace BarakaBg.Web.ViewModels.Orders
{
    using System.Globalization;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Mapping;

    public class OrderCheckViewModel : IMapFrom<Order>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string CreatedOn { get; set; }

        public OrderCondition Condition { get; set; }

        public PayForm PayForm { get; set; }

        public PayCondition PayCondition { get; set; }

        public decimal TotalPrice { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Order, OrderCheckViewModel>()
                .ForMember(
                    source => source.CreatedOn,
                    destination => destination.MapFrom(member => member.CreatedOn.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)));
        }
    }
}
