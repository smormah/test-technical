using System.Linq;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Exceptions;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<Country> GetCountryAsync(int id)
        {
            var countryDto = await _countryRepository.GetAsync(id);
            return new Country
            {
                Id = countryDto.Id,
                Name = countryDto.Name
            };
        }

        public async Task<int> GetCountryId(string name)
        {
            var countries = await _countryRepository.GetAllAsync();
            var found = countries.SingleOrDefault(x => x.Name == name);
            if (found == null)
                throw new NotFoundException(name);
            return found.Id;
        }
    }
}