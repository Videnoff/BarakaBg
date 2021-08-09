namespace BarakaBg.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.EntityFrameworkCore;

    public class ProductsService : IProductsService
    {
        private readonly string[] AllowedExtensions = new[] { "jpg", "jpeg", "png", "gif" };

        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IRepository<UserProductComment> userProductReviewRepository;
        private readonly IDeletableEntityRepository<Image> imagesRepository;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository,
            IDeletableEntityRepository<Image> imagesRepository,
            IRepository<UserProductComment> userProductReviewRepository)
        {
            this.productsRepository = productsRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.imagesRepository = imagesRepository;
            this.userProductReviewRepository = userProductReviewRepository;
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

        public async Task<bool> CreateReviewAsync<T>(T model)
        {
            var productReview = AutoMapperConfig.MapperInstance.Map<UserProductComment>(model);
            var product = this.GetById(productReview.ProductId);

            if (product == null || this.userProductReviewRepository.AllAsNoTracking().Any(x => x.ProductId == productReview.ProductId && x.UserId == productReview.UserId))
            {
                return false;
            }

            await this.userProductReviewRepository.AddAsync(productReview);
            await this.userProductReviewRepository.SaveChangesAsync();

            return true;
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

        public IEnumerable<T> GetRandom<T>(int count)
        {
            return this.productsRepository
                .All()
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .To<T>()
                .ToList();
        }

        public IEnumerable<T> GetNewest<T>(int productsToTake) =>
            this.productsRepository.AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .Take(productsToTake)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetTopRated<T>(int productsToTake)
        {
            var productIds = this.userProductReviewRepository.AllAsNoTracking()
                .GroupBy(x => x.ProductId)
                .Select(x => new
                {
                    ProductId = x.Key,
                    Total = x.Count(),
                    AvgRating = x.Average(r => r.Rating),
                })
                .OrderByDescending(x => x.AvgRating)
                .ThenByDescending(x => x.Total)
                .Take(productsToTake)
                .ToList();

            var products = new List<T>();

            foreach (var product in productIds)
            {
                var mappedProduct = this.GetById<T>(product.ProductId);
                products.Add(mappedProduct);
            }

            return products;
        }

        public int GetCount()
        {
            return this.productsRepository.All().Count();
        }

        public T GetById<T>(int id)
        {
            var product = this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            return product;
        }

        public async Task UpdateAsync(int id, EditProductInputModel inputModel)
        {
            var products = this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefault(x => x.Id == id);

            if (products != null)
            {
                products.Name = inputModel.Name;
                products.Brand = inputModel.Brand;
                products.ProductCode = inputModel.ProductCode;
                products.Stock = inputModel.Stock;
                products.Price = inputModel.Price;
                products.Description = inputModel.Description;
                products.CategoryId = inputModel.CategoryId;
            }

            await this.productsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetByIngredients<T>(IEnumerable<int> ingredientIds)
        {
            var query = this.productsRepository
                .All()
                .AsQueryable();

            foreach (var ingredientId in ingredientIds)
            {
                query = query
                    .Where(x => x.Ingredients
                        .Any(i => i.IngredientId == ingredientId));
            }

            return query
                .To<T>()
                .ToList();
        }

        public async Task DeleteAsync(int id)
        {
            var product = this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefault(x => x.Id == id);

            this.productsRepository.Delete(product);

            foreach (var image in product.Images)
            {
                this.imagesRepository.Delete(image);
            }

            await this.productsRepository.SaveChangesAsync();
            await this.imagesRepository.SaveChangesAsync();
        }

        public async Task<bool> UndeleteAsync(int id)
        {
            var product = this.GetDeletedById(id);

            if (product == null)
            {
                return false;
            }

            this.productsRepository.Undelete(product);
            await this.productsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteReviewAsync(string id)
        {
            var review = this.GetReviewById(id);
            if (review == null)
            {
                return false;
            }

            this.userProductReviewRepository.Delete(review);
            await this.userProductReviewRepository.SaveChangesAsync();

            return true;
        }

        public IEnumerable<T> GetAllDeleted<T>() =>
            this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .To<T>()
                .ToList();

        public bool HasProduct(int id) => this.productsRepository.AllAsNoTracking().Any(x => x.Id == id);

        public IEnumerable<T> GetAllProducts<T>(string searchTerm, int page, int productsToTake)
        {
            return this.productsRepository.AllAsNoTracking()
                .Skip((page - 1) * productsToTake)
                .Take(productsToTake)
                .To<T>().ToList();
        }

        private Product GetDeletedById(int id) =>
            this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefault(x => x.IsDeleted && x.Id == id);

        private Product GetById(int id) =>
            this.productsRepository.All()
                .Include(x => x.Images)
                .FirstOrDefault(x => x.Id == id);

        private UserProductComment GetReviewById(string id) =>
            this.userProductReviewRepository.All()
                .FirstOrDefault(x => x.Id == id);
    }
}
