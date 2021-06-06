namespace BarakaBg.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;

    public class ProductsService : IProductsService
    {
        private readonly string[] AllowedExtensions = new[] { "jpg", "jpeg", "png", "gif" };
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.productsRepository = productsRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateProductInputModel input, string userId, string imagePath)
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

            // /wwwroot/images/products/{id}.{ext}
            Directory.CreateDirectory($"{imagePath}/products/");
            foreach (var image in input.Images)
            {
                var extension = Path.GetExtension(image.FileName).TrimStart('.');
                if (!this.AllowedExtensions.Any(x => extension.EndsWith(x)))
                {
                    throw new Exception($"Invalid image format - {extension}!");
                }

                var dbImage = new Image
                {
                    AddedByUserId = userId,
                    Extension = extension,
                };

                product.Images.Add(dbImage);

                var physicalPath = $"{imagePath}/products/{dbImage.Id}.{extension}";
                using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                await image.CopyToAsync(fileStream);

                // TODO: Save image!
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

        public IEnumerable<T> GetRandom<T>(int random)
        {
            return this.productsRepository
                .All()
                .OrderBy(x => Guid.NewGuid())
                .To<T>()
                .ToList();
        }

        public int GetCount()
        {
            return this.productsRepository.All().Count();
        }

        public T GetById<T>(int id)
        {
            var product = this.productsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            return product;
        }
    }
}
