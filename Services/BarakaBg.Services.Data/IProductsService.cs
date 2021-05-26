namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Web.ViewModels.Products;

    public interface IProductsService
    {
        Task CreateAsync(CreateProductInputModel input, string userId, string imagePath);

        IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12);

        int GetCount();
    }
}
