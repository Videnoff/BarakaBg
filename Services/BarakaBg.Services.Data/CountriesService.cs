namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countryRepository;

        public CountriesService(IRepository<Country> countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public IEnumerable<Country> GetAll() => this.countryRepository
            .AllAsNoTracking()
            .ToList();
    }
}
