using System.Text;
using BarakaBg.Services.Messaging;

namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ProductsController : AdministrationController
    {
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;
        private readonly IEmailSender emailSender;

        public ProductsController(
            IDeletableEntityRepository<Product> productRepository,
            IProductsService productsService,
            ICategoriesService categoriesService,
            IEmailSender emailSender)
        {
            this.productRepository = productRepository;
            this.productsService = productsService;
            this.categoriesService = categoriesService;
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

        // GET: Administration/Products/Create
        public IActionResult Create()
        {
            this.ViewData["AddedByUserId"] = new SelectList(this.productRepository.All(), "Id", "Id");
            this.ViewData["CategoryId"] = new SelectList(this.productRepository.All(), "Id", "Id");
            return this.View();
        }

        // POST: Administration/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Name,Brand,ProductCode,Stock,Price,Description,Content,Feedback,OriginalUrl,CategoryId,AddedByUserId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")]
            Product product)
        {
            if (this.ModelState.IsValid)
            {
                await this.productRepository.AddAsync(product);
                await this.productRepository.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["AddedByUserId"] =
                new SelectList(this.productRepository.All(), "Id", "Id", product.AddedByUserId);
            this.ViewData["CategoryId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.CategoryId);
            return this.View(product);
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

            return this.RedirectToAction(nameof(this.ById), new {id});
        }

        // GET: Administration/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Administration/Products/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await this.productsService.DeleteAsync(id);
            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult ById(int id)
        {
            var product = this.productsService.GetById<SingleProductViewModel>(id);
            return this.View(product);
        }

        private bool ProductExists(int id)
        {
            return this.productRepository.All().Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> SendToEmail(int id)
        {
            var product = this.productsService.GetById<ProductInListViewModel>(id);
            var html = new StringBuilder();
            html.AppendLine($"<h1>{product.Name}</h1>");
            html.AppendLine($"<h3>{product.CategoryName}</h3>");
            html.AppendLine($"<img src=\"{product.ImageUrl}\" />");
            await this.emailSender.SendEmailAsync("baraka@barakabg.com", "BarakaBg", "ivvr41bqicex@marmaryta.email", product.Name, html.ToString());
            return this.RedirectToAction(nameof(this.ById));
        }
    }
}