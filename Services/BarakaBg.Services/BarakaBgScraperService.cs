namespace BarakaBg.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AngleSharp;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Models;

    public class BarakaBgScraperService : IBarakaBgScraperService
    {
        private readonly IConfiguration config;
        private readonly IBrowsingContext context;

        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IRepository<ProductIngredient> productIngredientsRepository;
        private readonly IDeletableEntityRepository<Image> imagesRepository;

        public BarakaBgScraperService(
            IDeletableEntityRepository<Category> categoriesRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository,
            IDeletableEntityRepository<Product> productsRepository,
            IRepository<ProductIngredient> productIngredientsRepository,
            IDeletableEntityRepository<Image> imagesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.productsRepository = productsRepository;
            this.productIngredientsRepository = productIngredientsRepository;
            this.imagesRepository = imagesRepository;
            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);
        }

        public async Task PopulateDbWithProductsAsync(int productsCount)
        {
            var concurrentBag = new ConcurrentBag<ProductDto>();

            for (var j = 61; j < 63; j++)
            {
                for (var i = 1148; i < productsCount; i++)
                {
                    try
                    {
                        var product = this.GetProduct(i, j);
                        concurrentBag.Add(product);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                foreach (var product in concurrentBag)
                {
                    var categoryId = await this.GetOrCreateCategoryAsync(product.CategoryName);
                    var productExists = this.productsRepository
                        .AllAsNoTracking()
                        .Any(x => x.Name == product.ProductName);

                    if (productExists)
                    {
                        continue;
                    }

                    var newProduct = new Product()
                    {
                        Name = product.ProductName,
                        Brand = product.Brand,
                        ProductCode = product.ProductCode,
                        Stock = product.Stock,
                        Description = product.ProductDescription,
                        Content = product.Content,
                        Price = product.Price,
                        OriginalUrl = product.OriginalUrl,
                        CategoryId = categoryId,
                    };

                    await this.productsRepository.AddAsync(newProduct);
                    await this.productsRepository.SaveChangesAsync();

                    foreach (var item in product.Ingredients)
                    {
                        if (item.Length < 2)
                        {
                            continue;
                        }

                        var ingredientId = await this.GetOrCreateIngredientAsync(item.Trim());

                        var productIngredient = new ProductIngredient()
                        {
                            IngredientId = ingredientId,
                            ProductId = newProduct.Id,
                        };

                        await this.productIngredientsRepository.AddAsync(productIngredient);
                        await this.productIngredientsRepository.SaveChangesAsync();
                    }

                    var image = new Image
                    {
                        RemoteImageUrl = product.ImageUrl,
                        ProductId = newProduct.Id,
                    };

                    await this.imagesRepository.AddAsync(image);
                    await this.imagesRepository.SaveChangesAsync();
                }
            }
        }

        private async Task<int> GetOrCreateIngredientAsync(string name)
        {
            var ingredient = this.ingredientsRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Name == name);

            if (ingredient == null)
            {
                ingredient = new Ingredient()
                {
                    Name = name,
                };

                await this.ingredientsRepository.AddAsync(ingredient);
                await this.ingredientsRepository.SaveChangesAsync();
            }

            return ingredient.Id;
        }

        private async Task<int> GetOrCreateCategoryAsync(string categoryName)
        {
            var category = this.categoriesRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Name == categoryName);

            if (category == null)
            {
                category = new Category()
                {
                    Name = categoryName,
                };

                await this.categoriesRepository.AddAsync(category);
                await this.categoriesRepository.SaveChangesAsync();
            }

            return category.Id;
        }

        private ProductDto GetProduct(int id, int path)
        {
            // Create a virtual request to specify the document to load (here from our fixed string)
            var document = this.context
                .OpenAsync($"http://baraka.bg/index.php?route=product/product&path={path}&product_id={id}")
                .GetAwaiter()
                .GetResult();

            if (document.StatusCode == HttpStatusCode.NotFound ||
                document.DocumentElement.OuterHtml.Contains("Продуктът не бе намерен!"))
            {
                throw new InvalidOperationException();
            }

            var product = new ProductDto();

            /*
             * Get price
             */
            var price = document.QuerySelector(".price").TextContent.Split(":", StringSplitOptions.RemoveEmptyEntries)
                .Last();
            var clearPrice = price.Trim();
            var lastPrice = clearPrice.Remove(clearPrice.Length - 3, 3).Replace(",", ".");
            product.Price = Math.Round(double.Parse(lastPrice), 2);

            /*
             * Get description
             */
            var description = document.GetElementById("tab-description").TextContent;
            product.ProductDescription = description;

            /*
             * Get content
             */
            try
            {
                if (document.GetElementById("tab-content").TextContent != null)
                {
                    var content = document.GetElementById("tab-content").TextContent;
                    product.Content = content;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            /*
             * TODO: Get Feedback
             */

            // var feedback = document.QuerySelector("tab-review").GetElementsByTagName("div");
            // foreach (var element in feedback)
            // {
            //    Console.WriteLine(element.TextContent);
            // }

            /*
             * Get Category Name
             */
            var productNameAndCategory = document
                .QuerySelectorAll("#brd-crumbs > ul")
                .Select(x => x.TextContent)
                .FirstOrDefault()
                ?.Split(" »", StringSplitOptions.RemoveEmptyEntries)
                .Reverse()
                .ToArray();

            if (productNameAndCategory != null)
            {
                product.CategoryName = productNameAndCategory[1].Trim();
            }

            /*
             * Get Image
             */
            var imageUrl = document.QuerySelector(".left.product-image > .image > img").GetAttribute("src");
            product.ImageUrl = imageUrl;

            /*
             * Get Ingredients
             */
            try
            {
                var ingredients = document.QuerySelector("#tab-content > p:nth-child(1)").TextContent.Split(", ");

                // TODO: Remove "Съдържание: "
                foreach (var item in ingredients)
                {
                    product.Ingredients.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            /*
             * Get Product Name
             */
            var productName = document.QuerySelector(".right > h1").TextContent;
            product.ProductName = productName;

            /*
             * Get tab elements
             */
            var tabElements = document.QuerySelectorAll(".product-description > tbody:nth-child(1) > tr");
            if (tabElements.Length == 3)
            {
                /*
                 * Get brand
                 */

                var brand = document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(2) > a:nth-child(1)")
                    .TextContent;
                product.Brand = brand;

                /*
                 * Get link
                 */
                try
                {
                    var linkToBrand = document.QuerySelector(
                            ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(2) > a:nth-child(1)").GetAttribute("href");
                    product.LinkToBrand = linkToBrand;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                /*
                 * Get Product Code
                */
                var productCode = document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)")
                    .TextContent;
                product.ProductCode = productCode;
                /*
                 * Get Stock
                 */
                var inStock = document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2)")
                    .TextContent;
                product.Stock = inStock;
            }
            else if (tabElements.Length == 2)
            {
                if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Марка:")
                {
                    try
                    {
                        if (document
                            .QuerySelector(
                                ".product-description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2)")
                            .TextContent != null)
                        {
                            var brand = document
                                .QuerySelector(
                                    ".product-description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2)")
                                .TextContent;
                            product.Brand = brand;

                            /*
                             * Get link
                             */
                            try
                            {
                                var linkToBrand = document.QuerySelector(
                                        ".product - description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2) > a")
                                    .GetAttribute("src");
                                product.LinkToBrand = linkToBrand;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Код на продукта:")
                {
                    /*
                     * Get Product Code
                    */
                    try
                    {
                        var productCode = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(2)")
                            .TextContent;
                        product.ProductCode = productCode;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Наличност:")
                {
                    /*
                     * Get Stock
                     */
                    try
                    {
                        var inStock = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2)")
                            .TextContent;
                        product.Stock = inStock;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (document.QuerySelector(".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(1) > span:nth-child(1)").TextContent == "Марка:")
                {
                    try
                    {
                        if (document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)")
                            .TextContent != null)
                        {
                            var brand = document
                                .QuerySelector(
                                    ".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)")
                                .TextContent;
                            product.Brand = brand;
                        }

                        /*
                         * Get link
                         */
                        try
                        {
                            var linkToBrand = document.QuerySelector(
                                    ".product - description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2) > a")
                                .GetAttribute("src");
                            product.LinkToBrand = linkToBrand;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document.QuerySelector(".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(1) > span:nth-child(1)").TextContent == "Код на продукта:")
                {
                    try
                    {
                        var productCode = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)")
                            .TextContent;
                        product.ProductCode = productCode;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document.QuerySelector(".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(1) > span:nth-child(1)").TextContent == "Наличност:")
                {
                    /*
                     * Get Stock
                     */
                    try
                    {
                        var inStock = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)")
                            .TextContent;
                        product.Stock = inStock;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            else if (tabElements.Length == 1)
            {
                if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Марка:")
                {
                    try
                    {
                        if (document
                            .QuerySelector(
                                ".product-description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2)")
                            .TextContent != null)
                        {
                            var brand = document
                                .QuerySelector(
                                    ".product-description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2)")
                                .TextContent;
                            product.Brand = brand;
                        }

                        /*
                         * Get link
                         */
                        try
                        {
                            var linkToBrand = document.QuerySelector(
                                    ".product - description > tbody:nth - child(1) > tr:nth - child(1) > td:nth - child(2) > a")
                                .GetAttribute("src");
                            product.LinkToBrand = linkToBrand;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Код на продукта:")
                {
                    /*
                     * Get Product Code
                    */
                    try
                    {
                        var productCode = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(2)")
                            .TextContent;
                        product.ProductCode = productCode;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (document
                    .QuerySelector(
                        ".product-description > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > span:nth-child(1)")
                    .TextContent == "Наличност:")
                {
                    /*
                     * Get Stock
                     */
                    try
                    {
                        var inStock = document
                            .QuerySelector(
                                ".product-description > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2)")
                            .TextContent;
                        product.Stock = inStock;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return product;
        }
    }
}
