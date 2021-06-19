namespace BarakaBg.Data.Models
{
    using System;
    using System.Collections.Generic;

    using BarakaBg.Data.Common.Models;

    public class Order : BaseDeletableModel<string>
    {
        public string UserFullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string AddressId { get; set; }

        public virtual Address Address { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentType PaymentType { get; set; }

        public string StripeId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime DeliveredOn { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public decimal DeliveryPrice { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}