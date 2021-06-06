namespace BarakaBg.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data;
    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ProductsController : AdministrationController
    {
        private readonly IDeletableEntityRepository<Product> productRepository;

        public ProductsController(IDeletableEntityRepository<Product> productRepository)
        {
            this.productRepository = productRepository;
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
        public async Task<IActionResult> Create([Bind("Name,Brand,ProductCode,Stock,Price,Description,Content,Feedback,OriginalUrl,CategoryId,AddedByUserId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Product product)
        {
            if (this.ModelState.IsValid)
            {
                this.productRepository.AddAsync(product);
                await this.productRepository.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["AddedByUserId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.AddedByUserId);
            this.ViewData["CategoryId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.CategoryId);
            return this.View(product);
        }

        // GET: Administration/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var product = this.productRepository.All().FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.NotFound();
            }

            this.ViewData["AddedByUserId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.AddedByUserId);
            this.ViewData["CategoryId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.CategoryId);
            return this.View(product);
        }

        // POST: Administration/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Brand,ProductCode,Stock,Price,Description,Content,Feedback,OriginalUrl,CategoryId,AddedByUserId,IsDeleted,DeletedOn,Id,CreatedOn,ModifiedOn")] Product product)
        {
            if (id != product.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.productRepository.Update(product);
                    await this.productRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.ProductExists(product.Id))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["AddedByUserId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.AddedByUserId);
            this.ViewData["CategoryId"] = new SelectList(this.productRepository.All(), "Id", "Id", product.CategoryId);
            return this.View(product);
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
            var product = this.productRepository.All().FirstOrDefault(x => x.Id == id);
            this.productRepository.Delete(product);
            await this.productRepository.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        private bool ProductExists(int id)
        {
            return this.productRepository.All().Any(e => e.Id == id);
        }
    }
}
