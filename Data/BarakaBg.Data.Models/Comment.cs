namespace BarakaBg.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class Comment : BaseDeletableModel<string>
    {
        public Comment()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public int Rating { get; set; }

        public string Content { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
