namespace BarakaBg.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class ProductComment : BaseModel<int>
    {
        public string Comments { get; set; }

        public DateTime PublishDate { get; set; }

        public byte Rating { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
