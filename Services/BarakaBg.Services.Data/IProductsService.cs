namespace BarakaBg.Services.Data
{
    using System.Threading.Tasks;

    using BarakaBg.Web.ViewModels.Products;

    public interface IProductsService
    {
        Task CreateAsync(CreateProductInputModel input);
    }
}