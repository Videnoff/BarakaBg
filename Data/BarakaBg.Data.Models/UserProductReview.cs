namespace BarakaBg.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class UserProductReview : BaseModel<string>
    {
        public UserProductReview()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public byte Rating { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
