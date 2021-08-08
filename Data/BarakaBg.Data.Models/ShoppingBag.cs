namespace BarakaBg.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    using BarakaBg.Data.Common.Models;

    public class ShoppingBag : BaseDeletableModel<string>
    {
        public ShoppingBag()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<ShoppingBagProduct> ShoppingBagProducts { get; set; }
    }
}
