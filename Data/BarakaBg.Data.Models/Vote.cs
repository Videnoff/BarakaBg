namespace BarakaBg.Data.Models
{
    using BarakaBg.Data.Common.Models;

    public class Vote : BaseModel<int>
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser U { get; set; }

        public byte Value { get; set; }
    }
}