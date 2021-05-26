namespace BarakaBg.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Models;

    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            await dbContext.Categories.AddAsync(new Category { Name = "Тяло" });
            await dbContext.Categories.AddAsync(new Category { Name = "Коса" });
            await dbContext.Categories.AddAsync(new Category { Name = "Лице" });
            await dbContext.Categories.AddAsync(new Category { Name = "Парфюми" });
            await dbContext.Categories.AddAsync(new Category { Name = "Аксесоари" });
            await dbContext.Categories.AddAsync(new Category { Name = "Серии" });
            await dbContext.Categories.AddAsync(new Category { Name = "Марки" });
            await dbContext.Categories.AddAsync(new Category { Name = "Био" });
            await dbContext.Categories.AddAsync(new Category { Name = "Сапуни" });

            await dbContext.SaveChangesAsync();
        }
    }
}
