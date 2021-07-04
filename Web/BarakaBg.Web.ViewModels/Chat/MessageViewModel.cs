namespace BarakaBg.Web.ViewModels.Chat
{
    using System.Globalization;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class MessageViewModel : IMapFrom<RoomMessage>, IHaveCustomMappings
    {
        public string CreatedOn { get; set; }

        public string Message { get; set; }

        public string SenderUsername { get; set; }

        public bool IsByRoomOwner { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<RoomMessage, MessageViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    y => y.MapFrom(m => m.CreatedOn.ToString(GlobalConstants.DateTimeFormat, CultureInfo.InvariantCulture)))
                .ForMember(
                    x => x.IsByRoomOwner,
                    y => y.MapFrom(m => m.SenderId == m.Room.OwnerId));
        }
    }
}
