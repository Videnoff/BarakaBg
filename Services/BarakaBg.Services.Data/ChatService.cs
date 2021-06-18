using BarakaBg.Data.Models;

namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;

    public class ChatService : IChatService
    {
        private readonly IDeletableEntityRepository<ChatRoom> chatRoomsRepository;
        private readonly IRepository<RoomMessage> roomMessagesRepository;

        public ChatService(
            IDeletableEntityRepository<ChatRoom> chatRoomsRepository,
            IRepository<RoomMessage> roomMessagesRepository)
        {
            this.chatRoomsRepository = chatRoomsRepository;
            this.roomMessagesRepository = roomMessagesRepository;
        }

        public Task<T> CreateRoomAsync<T>(string userId)
        {
            if (this.GetRoomIdByOwnerId(userId) != null)
            {
                return default;
            }

            var newRoom = new ChatRoom
            {
                OwnerId = userId,
            };

            await this.chatRoomsRepository.AddAsync(newRoom);
            await this.chatRoomsRepository.SaveChangesAsync();
        }

        public Task<T> AddMessageAsync<T>(string roomId, string message, string senderId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAllRooms<T>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAllMessagesByRoomId<T>(string roomId)
        {
            throw new System.NotImplementedException();
        }

        public string GetRoomIdByOwnerId(string ownerId)
        {
            throw new System.NotImplementedException();
        }
    }
}