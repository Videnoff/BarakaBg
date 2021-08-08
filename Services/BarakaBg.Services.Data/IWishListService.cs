namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWishListService
    {
        public Task<bool> AddAsync(int productId, string userId);

        public Task<bool> DeleteAsync(int productId, string userId);

        public IEnumerable<T> GetAll<T>(string userId);

        public int GetCount(string userId);
    }
}
