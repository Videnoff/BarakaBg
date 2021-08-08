namespace BarakaBg.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class ShoppingBagProduct : BaseDeletableModel<string>
    {
        public ShoppingBagProduct()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ShoppingBagId { get; set; }

        public virtual ShoppingBag ShoppingBag { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
