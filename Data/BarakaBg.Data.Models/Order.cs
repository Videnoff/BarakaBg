namespace BarakaBg.Data.Models
{
    using System;
    using System.Collections.Generic;

    using BarakaBg.Data.Common.Models;
    using BarakaBg.Data.Models.Enums;

    public class Order : BaseDeletableModel<string>
    {
        public Order()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Products = new HashSet<ProductOrder>();
            this.IsDelivered = false;
            this.PayCondition = PayCondition.Unpaid;
            this.Condition = OrderCondition.Processing;
        }

        public string UserFullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string AddressId { get; set; }

        public virtual Address Address { get; set; }

        public PayCondition PayCondition { get; set; }

        public PayForm PayForm { get; set; }

        public string StripeId { get; set; }

        public OrderCondition Condition { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime? DeliveredOn { get; set; }

        public ShipmentType ShipmentType { get; set; }

        public decimal DeliveryPrice { get; set; }

        public virtual ICollection<ProductOrder> Products { get; set; }
    }
}
