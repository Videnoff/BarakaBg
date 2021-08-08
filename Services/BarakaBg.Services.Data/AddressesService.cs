﻿namespace BarakaBg.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using BarakaBg.Web.ViewModels.Addresses;

    public class AddressesService : IAddressesService
    {
        private readonly IDeletableEntityRepository<Address> addressRepository;
        private readonly IRepository<City> citiesRepository;

        public AddressesService(
            IDeletableEntityRepository<Address> addressRepository,
            IRepository<City> citiesRepository)
        {
            this.addressRepository = addressRepository;
            this.citiesRepository = citiesRepository;
        }

        public async Task<bool> CreateAsync(AddressInputModel model)
        {
            var address = new Address
            {
                Street = model.Street,
                Description = model.Description,
                UserId = model.UserId,
            };

            var city = this.citiesRepository.All()
                .FirstOrDefault(x =>
                    x.Name == model.City && x.ZipCode == model.ZipCode && x.CountryId == model.CountryId);

            if (city == null)
            {
                city = new City
                {
                    Name = model.City,
                    ZipCode = model.ZipCode,
                    CountryId = model.CountryId,
                };

                await this.citiesRepository.AddAsync(city);
            }

            address.City = city;

            var addressExists = this.addressRepository.AllAsNoTracking()
                .Any(x => x.Street == address.Street && x.Description == address.Description && x.CityId == city.Id);

            if (addressExists)
            {
                return false;
            }

            await this.addressRepository.AddAsync(address);
            await this.addressRepository.SaveChangesAsync();

            return true;
        }

        public IEnumerable<T> GetAll<T>(string userId) => this.addressRepository
            .AllAsNoTracking()
            .Where(x => x.UserId == userId)
            .To<T>()
            .ToList();

        public async Task<bool> DeleteAsync(string id)
        {
            var address = this.addressRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == id);

            if (address == null)
            {
                return false;
            }

            this.addressRepository.Delete(address);
            await this.addressRepository.SaveChangesAsync();

            return true;
        }
    }
}
