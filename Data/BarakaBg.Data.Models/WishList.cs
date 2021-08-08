namespace BarakaBg.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class WishList : BaseModel<string>
    {
        public WishList()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
