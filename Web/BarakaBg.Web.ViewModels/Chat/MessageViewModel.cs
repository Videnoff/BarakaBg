namespace BarakaBg.Web.ViewModels.Chat
{
    using System.Globalization;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class MessageViewModel : IMapFrom<ChatRoomMessage>, IHaveCustomMappings
    {
        public string CreatedOn { get; set; }

        public bool IsByRoomOwner { get; set; }

        public string SenderUserName { get; set; }

        public string Message { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ChatRoomMessage, MessageViewModel>()
                .ForMember(
                    source => source.CreatedOn,
                    destination => destination.MapFrom(member => member.CreatedOn.ToString(GlobalConstants.ParsedDate, CultureInfo.InvariantCulture)))
                .ForMember(
                    source => source.IsByRoomOwner,
                    destination => destination.MapFrom(member => member.SenderId == member.Room.OwnerId));
        }
    }
}
