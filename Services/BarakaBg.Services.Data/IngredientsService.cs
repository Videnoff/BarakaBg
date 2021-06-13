using System.Collections.Generic;
using System.Linq;
using BarakaBg.Data.Common.Repositories;
using BarakaBg.Data.Models;
using BarakaBg.Services.Mapping;

namespace BarakaBg.Services.Data
{
    public class IngredientsService : IIngredientsService
    {
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public IngredientsService(IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.ingredientsRepository = ingredientsRepository;
        }

        public IEnumerable<T> GetAll<T>()
        {
            return this.ingredientsRepository.All().To<T>().ToList();
        }
    }
}