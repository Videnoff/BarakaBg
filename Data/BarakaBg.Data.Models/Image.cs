namespace BarakaBg.Data.Models
{
    using System;

    using BarakaBg.Data.Common.Models;

    public class Image : BaseDeletableModel<string>
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string AddedByUserId { get; set; }

        public ApplicationUser AddedByUser { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string Extension { get; set; }

        // The contents of the Image is in the file system

        public string RemoteImageUrl { get; set; }
    }
}
