namespace BarakaBg.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
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

        public async Task CreateAsync(CreateProductInputModel input)
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
    }
}