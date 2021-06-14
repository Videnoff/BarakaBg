namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BarakaBg.Web.ViewModels.Products;

    public interface IProductsService
    {
        Task CreateAsync(CreateProductInputModel input, string userId, string imagePath);

        IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12);

        IEnumerable<T> GetRandom<T>(int random);

        int GetCount();

        T GetById<T>(int id);

        Task UpdateAsync(int id, EditProductInputModel inputModel);

        IEnumerable<T> GetByIngredients<T>(IEnumerable<int> ingredientIds);
    }
}
