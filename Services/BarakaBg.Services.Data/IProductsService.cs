namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;
    using BarakaBg.Web.ViewModels.Products;

    public interface IProductsService
    {
        Task CreateAsync(CreateProductInputModel input, string userId, string imagePath);

        public Task<bool> CreateReviewAsync<T>(T model);

        IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12);

        IEnumerable<T> GetRandom<T>(int random);

        public IEnumerable<T> GetNewest<T>(int productsToTake);

        public IEnumerable<T> GetTopRated<T>(int productsToTake);

        int GetCount();

        T GetById<T>(int id);

        Task UpdateAsync(int id, EditProductInputModel inputModel);

        IEnumerable<T> GetByIngredients<T>(IEnumerable<int> ingredientIds);

        Task DeleteAsync(int id);

        public Task<bool> UndeleteAsync(int id);

        public Task<bool> DeleteReviewAsync(string id);

        public IEnumerable<T> GetAllDeleted<T>();

        public bool HasProduct(int id);

        public IEnumerable<Product> Search(string searchTerm);
    }
}
