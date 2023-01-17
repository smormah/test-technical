using System.Linq;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAddressService _addressService;

        public CustomerService(ICustomerRepository customerRepository, IAddressService addressService)
        {
            _customerRepository = customerRepository;
            _addressService = addressService;
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            var customerDto = await _customerRepository.GetAsync(id);
            if (customerDto == null)
                return null;

            var address = await _addressService.GetAddressAsync(customerDto.AddressId);
            return MapToCustomer(customerDto, address);
        }

        public async Task<int> AddNewCustomer(Customer customer)
        {
            var address = MapToAddress(customer);
            var addressId = await _addressService.AddNewAddress(address);

            var customerId = await _customerRepository.GenerateIdentityAsync();
            var customerDto = MapToCustomerDto(customerId, customer, addressId);

            await _customerRepository.InsertAsync(customerDto);
            return customerId;
        }

        private static Customer MapToCustomer(CustomerDto customerDto, Address address)
        {
            return new Customer
            {
                Id = customerDto.Id,
                FullName = $"{customerDto.FirstName} {customerDto.LastName}".Trim(),
                Street = address?.Street,
                City = address?.City,
                Country = address?.Country,
                Postcode = address?.Postcode
            };
        }

        private static CustomerDto MapToCustomerDto(int customerId, Customer customer, int addressId)
        {
            var names = customer.FullName.Split(" ");
            return new CustomerDto
            {
                Id = customerId,
                FirstName = names.First(),
                LastName = names.Length > 1 ? names.Last() : null,
                AddressId = addressId
            };
        }

        private static Address MapToAddress(Customer customer)
        {
            return new Address
            {
                Street = customer.Street,
                City = customer.City,
                Postcode = customer.Postcode,
                Country = customer.Country
            } ;
        }
    }
}
