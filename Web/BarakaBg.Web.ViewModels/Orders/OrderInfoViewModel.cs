namespace BarakaBg.Web.ViewModels.Orders
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Data.Models.Enums;
    using BarakaBg.Services.Mapping;

    public class OrderInfoViewModel : IMapFrom<Order>, IHaveCustomMappings
    {
        [Display(Name = "Order Id")]
        public string Id { get; set; }

        [Display(Name = "Full Name")]
        public string UserFullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public decimal DeliveryPrice { get; set; }

        [Display(Name = "Total Price (USD)")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Created On")]
        public string CreatedOn { get; set; }

        [Display(Name = "Payment Form")]
        public PayForm PayForm { get; set; }

        [Display(Name = "Payment Condition")]
        public PayCondition PayCondition { get; set; }

        public bool IsDelivered { get; set; }

        [Display(Name = "Delivered On")]
        public string DeliveredOn { get; set; }

        [Display(Name = "Order Condition")]
        public OrderCondition OrderCondition { get; set; }

        [Display(Name = "Shipment Type")]
        public ShipmentType ShipmentType { get; set; }

        [Display(Name = "Stripe Payment Id")]
        public string StripeId { get; set; }

        public IEnumerable<OrderProductsViewModel> Products { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Order, OrderInfoViewModel>()
                .ForMember(
                    source => source.Address,
                    destination => destination.MapFrom(member => $"{member.Address.Street} {member.Address.City.Name}, {member.Address.City.ZipCode}, {member.Address.City.Country.Name}"))
                .ForMember(
                    source => source.CreatedOn,
                    destination => destination.MapFrom(member => member.CreatedOn.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)))
                .ForMember(
                    source => source.DeliveredOn,
                    destination => destination.MapFrom(member => (member.DeliveredOn == null) ? null : member.DeliveredOn.Value.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)));
        }
    }
}
