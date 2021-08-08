namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;

    using BarakaBg.Data.Models;

    public interface ICountriesService
    {
        public IEnumerable<Country> GetAll();
    }
}
