namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;

    public class IngredientsService : IIngredientsService
    {
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public IngredientsService(IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.ingredientsRepository = ingredientsRepository;
        }

        public IEnumerable<T> GetAllPopular<T>()
        {
            return this.ingredientsRepository
                .All()
                .Where(x => x.Products.Count >= 10)
                .OrderBy(x => x.Name)
                .To<T>()
                .ToList();
        }
    }
}
