namespace BarakaBg.Web.ViewModels.Chat
{
    using System.Globalization;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Common;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class RoomViewModel : IMapFrom<ChatRoom>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string CreatedOn { get; set; }

        public string OwnerUserName { get; set; }

        public string LastMessage { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {

            configuration.CreateMap<ChatRoom, RoomViewModel>()
                .ForMember(
                    x => x.CreatedOn,
                    y => y.MapFrom(member =>
                        member.CreatedOn.ToString(GlobalConstants.DateTimeFormat, CultureInfo.InvariantCulture)))
                .ForMember(
                    x => x.LastMessage,
                    y => y.MapFrom(member =>
                        (!member.Messages.Any())
                            ? null
                            : member.Messages.OrderByDescending(x => x.CreatedOn).First().Message));
        }
    }
}
