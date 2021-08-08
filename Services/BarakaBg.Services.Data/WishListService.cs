namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using Microsoft.AspNetCore.Identity;

    public class WishListService : IWishListService
    {
        private readonly IRepository<WishList> wishListRepository;
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public WishListService(
            IRepository<WishList> wishListRepository,
            IDeletableEntityRepository<Product> productsRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.wishListRepository = wishListRepository;
            this.productsRepository = productsRepository;
            this.userManager = userManager;
        }

        public async Task<bool> AddAsync(int productId, string userId)
        {
            var product = this.GetProductById(productId);

            if (product == null)
            {
                return false;
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await this.wishListRepository.AddAsync(new WishList
            {
                ProductId = product.Id,
                UserId = user.Id,
            });

            await this.wishListRepository.SaveChangesAsync();
            return false;
        }

        public async Task<bool> DeleteAsync(int productId, string userId)
        {
            var product = this.GetProductById(productId);

            if (product == null)
            {
                return false;
            }

            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var favoriteProduct = this.GetFavoriteProductById(productId, userId);

            if (favoriteProduct == null)
            {
                return false;
            }

            this.wishListRepository.Delete(favoriteProduct);
            await this.wishListRepository.SaveChangesAsync();

            return true;
        }

        public IEnumerable<T> GetAll<T>(string userId) =>
            this.wishListRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .To<T>()
                .ToList();

        public int GetCount(string userId) => this.wishListRepository
            .AllAsNoTracking()
            .Count(x => x.UserId == userId);

        private Product GetProductById(int id) =>
        this.productsRepository.AllAsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        private WishList GetFavoriteProductById(int productId, string userId) =>
            this.wishListRepository.AllAsNoTracking()
                .FirstOrDefault(x => x.ProductId == productId && x.UserId == userId);
    }
}
