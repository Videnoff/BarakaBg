namespace BarakaBg.Services
{
    using System.Threading.Tasks;

    public interface IBarakaBgScraperService
    {
        Task PopulateDbWithProductsAsync(int productsCount);
    }
}