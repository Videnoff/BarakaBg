namespace BarakaBg.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Common;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.Infrastructure;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    public class ShoppingBagService : IShoppingBagService
    {
        private readonly IRepository<ShoppingBagProduct> shoppingBagProductRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductsService productsService;

        public ShoppingBagService(
            IRepository<ShoppingBagProduct> shoppingBagProductRepository,
            UserManager<ApplicationUser> userManager,
            IProductsService productsService)
        {
            this.shoppingBagProductRepository = shoppingBagProductRepository;
            this.userManager = userManager;
            this.productsService = productsService;
        }

        public async Task<bool> AddProductAsync(bool isUserAuthenticated, ISession session, string userId, int productId, int quantity = 1)
        {
            if (isUserAuthenticated)
            {
                var user = await this.userManager.FindByIdAsync(userId);
                var shoppingBagId = user.ShoppingBagId;

                var shoppingBagExists = this.GetShoppingBagByIdAndProductId(productId, shoppingBagId) != null;

                if (shoppingBagExists)
                {
                    return false;
                }

                var productExists = this.productsService.HasProduct(productId);

                if (!productExists)
                {
                    return false;
                }

                var newShoppingBag = new ShoppingBagProduct
                {
                    ShoppingBagId = shoppingBagId,
                    ProductId = productId,
                    Quantity = quantity,
                };

                await this.shoppingBagProductRepository.AddAsync(newShoppingBag);
                await this.shoppingBagProductRepository.SaveChangesAsync();

                return true;
            }
            else
            {
                var shoppingBagSession = session.GetObjectFromJson<List<ShoppingBagProductViewModel>>(GlobalConstants.SessionShoppingBagKey);

                if (shoppingBagSession == null)
                {
                    shoppingBagSession = new List<ShoppingBagProductViewModel>();
                }

                if (shoppingBagSession.Any(x => x.ProductId == productId))
                {
                    return false;
                }

                var product = this.productsService.GetById<SingleProductViewModel>(productId);
                var shoppingCartProduct = new ShoppingBagProductViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    ImageUrl = product.ImageUrl,
                    AverageRating = product.AverageVote,
                    Quantity = quantity,
                };

                shoppingBagSession.Add(shoppingCartProduct);

                session.SetObjectAsJson(GlobalConstants.SessionShoppingBagKey, shoppingBagSession);

                return true;
            }
        }

        public async Task<int> GetProductsCount(bool isUserAuthenticated, ISession session, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingCardId = user.ShoppingBagId;

            return this.shoppingBagProductRepository
                .AllAsNoTracking()
                .Count(x => x.ShoppingBagId == shoppingCardId);
        }

        public async Task<bool> AnyProductsAsync(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingBagId = user.ShoppingBagId;

            return this.shoppingBagProductRepository
                .AllAsNoTracking()
                .Any(x => x.ShoppingBagId == shoppingBagId);
        }

        public async Task<bool> UpdateQuantityAsync(bool isUserAuthenticated, ISession session, string userId, int productId, bool increase)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingBagId = user.ShoppingBagId;

            var shoppingBag = this.GetShoppingBagByIdAndProductId(productId, shoppingBagId);

            if (shoppingBag == null)
            {
                return false;
            }

            var quantity = shoppingBag.Quantity;
            if (increase)
            {
                quantity++;
            }
            else
            {
                quantity = Math.Max(quantity - 1, 1);
            }

            shoppingBag.Quantity = quantity;

            this.shoppingBagProductRepository.Update(shoppingBag);
            await this.shoppingBagProductRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<T>> GetAllProductsAsync<T>(bool isUserAuthenticated, ISession session, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingBagId = user.ShoppingBagId;

            return this.shoppingBagProductRepository
                .AllAsNoTracking()
                .Where(x => x.ShoppingBagId == shoppingBagId)
                .To<T>()
                .ToList();
        }

        public async Task<bool> DeleteProductAsync(bool isUserAuthenticated, ISession session, string userId, int productId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingBagId = user.ShoppingBagId;

            var shoppingBag = this.GetShoppingBagByIdAndProductId(productId, shoppingBagId);

            if (shoppingBag == null)
            {
                return false;
            }

            this.shoppingBagProductRepository.Delete(shoppingBag);
            await this.shoppingBagProductRepository.SaveChangesAsync();

            return true;
        }

        public async Task DeleteAllProductsAsync(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var shoppingBagId = user.ShoppingBagId;

            var products = this.shoppingBagProductRepository.All()
                .Where(x => x.ShoppingBagId == shoppingBagId)
                .ToList();

            foreach (var product in products)
            {
                this.shoppingBagProductRepository.Delete(product);
            }

            await this.shoppingBagProductRepository.SaveChangesAsync();
        }

        private ShoppingBagProduct GetShoppingBagByIdAndProductId(int productId, string shoppingBagId) =>
            this.shoppingBagProductRepository
                .All()
                .FirstOrDefault(x => x.ShoppingBagId == shoppingBagId && x.ProductId == productId);
    }
}
