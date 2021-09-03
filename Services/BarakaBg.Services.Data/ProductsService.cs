namespace BarakaBg.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    public class ProductsService : IProductsService
    {
        private readonly string[] AllowedExtensions = new[] { "jpg", "jpeg", "png", "gif" };

        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IImagesService imagesService;
        private readonly IDeletableEntityRepository<Image> imagesRepository;
        private readonly IRepository<ProductComment> productCommentRepository;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            IImagesService imagesService,
            IDeletableEntityRepository<Image> imagesRepository,
            IRepository<ProductComment> productCommentRepository)
        {
            this.productsRepository = productsRepository;
            this.imagesService = imagesService;
            this.imagesRepository = imagesRepository;
            this.productCommentRepository = productCommentRepository;
        }

        public async Task CreateAsync<T>(T model, IEnumerable<IFormFile> images, string fullDirectoryPath, string webRootPath)
        {
            var product = AutoMapperConfig.MapperInstance.Map<Product>(model);

            if (images != null && images.Count() > 0)
            {
                foreach (var image in images)
                {
                    var imageUrl = await this.imagesService.UploadLocalImageAsync(image, fullDirectoryPath);
                    product.Images.Add(new Image
                    {
                        RemoteImageUrl = imageUrl.Replace(webRootPath, string.Empty).Replace("\\", "/"),
                    });
                }
            }

            await this.productsRepository.AddAsync(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public async Task<bool> CreateReviewAsync<T>(T model)
        {
            var productReview = AutoMapperConfig.MapperInstance.Map<ProductComment>(model);
            var product = this.GetById(productReview.ProductId);

            if (product == null || this.productCommentRepository.AllAsNoTracking().Any(x => x.ProductId == productReview.ProductId))
            {
                return false;
            }

            await this.productCommentRepository.AddAsync(productReview);
            await this.productCommentRepository.SaveChangesAsync();

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

        //public async Task<bool> DeleteReviewAsync(string id)
        //{
        //    var review = this.GetReviewById(id);
        //    if (review == null)
        //    {
        //        return false;
        //    }

        //    this.userProductReviewRepository.Delete(review);
        //    await this.userProductReviewRepository.SaveChangesAsync();

        //    return true;
        //}
        public IEnumerable<T> GetAllDeleted<T>() =>
            this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .Where(x => x.IsDeleted)
                .To<T>()
                .ToList();

        public bool HasProduct(int id) => this.productsRepository.AllAsNoTracking().Any(x => x.Id == id);

        public IEnumerable<Product> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return this.productsRepository.All();
            }

            return this.productsRepository.All().Where(x => x.Name.Contains(searchTerm) || x.Category.Name.Contains(searchTerm));
        }

        private Product GetDeletedById(int id) =>
            this.productsRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefault(x => x.IsDeleted && x.Id == id);

        private Product GetById(int id) =>
            this.productsRepository.All()
                .Include(x => x.Images)
                .FirstOrDefault(x => x.Id == id);

        //private UserProductReview GetReviewById(string id) =>
        //    this.userProductReviewRepository.All()
        //        .FirstOrDefault(x => x.Id == id);
    }
}
