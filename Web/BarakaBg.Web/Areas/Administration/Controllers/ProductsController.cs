
namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BarakaBg.Data;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Services.Messaging;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ProductsController : AdministrationController
    {
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;
        private readonly IEmailSender emailSender;

        public ProductsController(
            IDeletableEntityRepository<Product> productRepository,
            IProductsService productsService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            IEmailSender emailSender)
        {
            this.productRepository = productRepository;
            this.productsService = productsService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
            this.environment = environment;
            this.emailSender = emailSender;
        }

        // GET: Administration/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = this.productRepository
                .AllWithDeleted()
                .Include(p => p.AddedByUser)
                .Include(p => p.Category);

            return this.View(await applicationDbContext.ToListAsync());
        }

        // GET: Administration/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var product = await this.productRepository
                .All()
                .Include(p => p.AddedByUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return this.NotFound();
            }

            return this.View(product);
        }

        public IActionResult Create()
        {
            var viewModel = new CreateProductInputModel
            {
                CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
                return this.View(input);
            }

            // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await this.userManager.GetUserAsync(this.User);
            try
            {
                await this.productsService.CreateAsync(input, user.Id, $"{this.environment.WebRootPath}/images");
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
                input.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
                return this.View(input);
            }

            this.TempData["Message"] = "Product added successfully.";

            // return this.Json(input);
            return this.RedirectToAction(nameof(this.All), "Products", new { area = string.Empty });
        }

        // GET: Administration/Products/Edit/5
        public IActionResult Edit(int id)
        {
            var inputModel = this.productsService.GetById<EditProductInputModel>(id);
            inputModel.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
            return this.View(inputModel);
        }

        // POST: Administration/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductInputModel input)
        {
            if (id != input.Id)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                input.Id = id;
                input.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();
                return this.View(input);
            }

            await this.productsService.UpdateAsync(id, input);

            return this.RedirectToAction(nameof(this.ById), new { area = string.Empty, id });
        }

        // GET: Administration/Products/Delete/5
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var product = await this.productRepository
                .All()
                .Include(p => p.AddedByUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return this.NotFound();
            }

            this.productRepository.Delete(product);

            await this.productRepository.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.All), "Products", new { area = string.Empty });
        }

        // Administration/Products/Delete/5
        public async Task<IActionResult> HardDelete(int id)
        {
            var product = await this.productRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == id);

            await this.productsService.DeleteAsync(id);

            await this.productRepository.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.All), "Products", new { area = string.Empty });
        }

        public async Task<IActionResult> Undelete(int id)
        {
            var undeleteResult = await this.productsService.UndeleteAsync(id);

            if (undeleteResult)
            {
                this.TempData["Alert"] = "Successfully restored product";
            }
            else
            {
                this.TempData["Error"] = "There was a problem restoring the product.";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult All(int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 12;
            var viewModel = new ProductsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                ProductsCount = this.productsService.GetCount(),
                Products = this.productsService.GetAll<ProductInListViewModel>(id, ItemsPerPage),
            };

            return this.View(viewModel);
        }

        public IActionResult ById(int id)
        {
            var product = this.productsService.GetById<SingleProductViewModel>(id);
            return this.View(product);
        }

        [HttpPost]
        public async Task<IActionResult> SendToEmail(int id)
        {
            var product = this.productsService.GetById<ProductInListViewModel>(id);
            var html = new StringBuilder();
            html.AppendLine($"<h1>{product.Name}</h1>");
            html.AppendLine($"<h3>{product.CategoryName}</h3>");
            html.AppendLine($"<img src=\"{product.ImageUrl}\" />");
            await this.emailSender.SendEmailAsync("videnoff@students.softuni.bg", "BarakaBg", "sabina.draganova@gmail.com", product.Name, html.ToString());
            return this.RedirectToAction(nameof(this.ById), "Products", new { area = string.Empty, id });
        }

        private bool ProductExists(int id)
        {
            return this.productRepository.All().Any(e => e.Id == id);
        }
    }
}
