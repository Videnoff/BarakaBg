namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.productsRepository = productsRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateProductInputModel input, string userId)
        {
            var product = new Product
            {
                CategoryId = input.CategoryId,
                Description = input.Description,
                Brand = input.Brand,
                Name = input.Name,
                Price = input.Price,
                Stock = input.Stock,
                Content = input.Content,
                ProductCode = input.ProductCode,
                AddedByUserId = userId,
            };

            foreach (var inputIngredient in input.Ingredients)
            {
                var ingredient = this.ingredientsRepository.All().FirstOrDefault(x => x.Name == inputIngredient.IngredientName);

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Name = inputIngredient.IngredientName,
                    };
                }

                product.Ingredients.Add(new ProductIngredient
                {
                    Ingredient = ingredient,
                });
            }

            await this.productsRepository.AddAsync(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12)
        {
            /*
             * 1-12  - page 1       0      (page - 1) * itemsPerPage
             * 13-24  - page 1     12      (page - 2) * itemsPerPage
             * 25-36  - page 1     24      (page - 3) * itemsPerPage
             */

            var products = this.productsRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

            return products;
        }

        public int GetCount()
        {
            return this.productsRepository.All().Count();
        }
    }
}
