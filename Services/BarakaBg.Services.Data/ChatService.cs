namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class ChatService : IChatService
    {
        private readonly IDeletableEntityRepository<ChatRoom> chatRoomRepository;
        private readonly IRepository<ChatRoomMessage> roomMessageRepository;

        public ChatService(
            IDeletableEntityRepository<ChatRoom> chatRoomRepository,
            IRepository<ChatRoomMessage> roomMessageRepository)
        {
            this.chatRoomRepository = chatRoomRepository;
            this.roomMessageRepository = roomMessageRepository;
        }

        public async Task<T> CreateRoomAsync<T>(string userId)
        {
            if (this.GetRoomIdByOwnerId(userId) != null)
            {
                return default;
            }

            var newRoom = new ChatRoom
            {
                OwnerId = userId,
            };

            await this.chatRoomRepository.AddAsync(newRoom);
            await this.chatRoomRepository.SaveChangesAsync();

            return this.GetRoomById<T>(newRoom.Id);
        }

        public async Task<T> AddMessageAsync<T>(string roomId, string message, string senderId)
        {
            var room = this.GetRoomById(roomId);

            if (room == null)
            {
                return default;
            }

            var roomMessage = new ChatRoomMessage
            {
                Message = message,
                SenderId = senderId,
            };

            room.Messages.Add(roomMessage);
            await this.chatRoomRepository.SaveChangesAsync();

            return AutoMapperConfig.MapperInstance.Map<T>(roomMessage);
        }

        public IEnumerable<T> GetAllRooms<T>() =>
            this.chatRoomRepository.AllAsNoTracking()
                .To<T>().ToList();

        public IEnumerable<T> GetAllMessagesByRoomId<T>(string roomId) =>
            this.roomMessageRepository.AllAsNoTracking()
                .Where(x => x.RoomId == roomId)
                .To<T>().ToList();

        public string GetRoomIdByOwnerId(string ownerId) =>
            this.chatRoomRepository.AllAsNoTracking()
                .FirstOrDefault(x => x.OwnerId == ownerId)?.Id;

        private ChatRoom GetRoomById(string roomId) =>
            this.chatRoomRepository.All()
                .FirstOrDefault(x => x.Id == roomId);

        private T GetRoomById<T>(string roomId) =>
            this.chatRoomRepository.AllAsNoTracking()
                .Where(x => x.Id == roomId)
                .To<T>().FirstOrDefault();
    }
}
