namespace BarakaBg.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class ProductOrder : BaseModel<int>
    {
        [Required]
        public string OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
