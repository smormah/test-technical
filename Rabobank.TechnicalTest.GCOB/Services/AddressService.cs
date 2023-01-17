using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICountryService _countryService;

        public AddressService(IAddressRepository addressRepository, ICountryService countryService)
        {
            _addressRepository = addressRepository;
            _countryService = countryService;
        }

        public async Task<Address> GetAddressAsync(int id)
        {
            var addressDto = await _addressRepository.GetAsync(id);
            if (addressDto == null)
                return null;

            var country = await _countryService.GetCountryAsync(addressDto.CountryId);
            return new Address
            {
                Id = addressDto.Id,
                Street = addressDto.Street,
                City = addressDto.City,
                Postcode = addressDto.Postcode,
                Country = country?.Name
            };
        }

        public async Task<int> AddNewAddress(Address address)
        {
            var countryId = await _countryService.GetCountryId(address.Country);

            var addressId = await _addressRepository.GenerateIdentityAsync();
            var addressDto = MapToAddressDto(addressId, address, countryId);

            await _addressRepository.InsertAsync(addressDto);
            return addressId;
        }

        private AddressDto MapToAddressDto(int addressId, Address address, int countryId)
        {
            return new AddressDto
            {
                Id = addressId,
                Street = address.Street,
                City = address.City,
                Postcode = address.Postcode,
                CountryId = countryId
            };
        }
    }
}