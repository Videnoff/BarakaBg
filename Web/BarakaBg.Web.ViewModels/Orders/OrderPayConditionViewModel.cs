namespace BarakaBg.Web.ViewModels.Orders
{
    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Mapping;

    public class OrderPayConditionViewModel : IMapFrom<Order>
    {
        public string Id { get; set; }

        public PayCondition PayCondition { get; set; }
    }
}
