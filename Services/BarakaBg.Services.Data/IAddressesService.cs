namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Web.ViewModels.Addresses;

    public interface IAddressesService
    {
        public Task<bool> CreateAsync(AddressInputModel model);

        public IEnumerable<T> GetAll<T>(string userId);

        public Task<bool> DeleteAsync(string id);
    }
}
