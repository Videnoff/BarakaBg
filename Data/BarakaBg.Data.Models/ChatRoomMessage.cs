namespace BarakaBg.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using BarakaBg.Data.Common.Models;

    public class ChatRoomMessage : BaseModel<int>
    {
        [Required]
        public string RoomId { get; set; }

        public virtual ChatRoom Room { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }
    }
}
