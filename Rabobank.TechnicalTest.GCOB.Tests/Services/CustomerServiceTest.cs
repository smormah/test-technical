using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerServiceTest
    {
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IAddressService> _addressServiceMock;
        private readonly Random _rnd = new Random();

        [TestInitialize]
        public void Initialize()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>(MockBehavior.Strict);
            _addressServiceMock = new Mock<IAddressService>(MockBehavior.Strict);
        }

        [TestMethod]
        public async Task GivenHaveACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsReturned()
        {
            // Arrange
            var customerId = CreateRandomNumber();

            var returnedAddress = new Address
            {
                Id = CreateRandomNumber(),
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            var returnedCustomer = new CustomerDto
            {
                Id = customerId,
                FirstName = "John",
                LastName = "Doe",
                AddressId = returnedAddress.Id
            };

            _customerRepositoryMock.Setup(x => x.GetAsync(customerId)).ReturnsAsync(returnedCustomer);
            _addressServiceMock.Setup(x => x.GetAddressAsync(returnedAddress.Id)).ReturnsAsync(returnedAddress);

            // Act
            var customer = await CreateCustomerService().GetCustomerAsync(customerId);

            // Assert
            Assert.IsNotNull(customer);;
            Assert.AreEqual(returnedCustomer.Id, customer.Id);
            Assert.AreEqual($"{returnedCustomer.FirstName} {returnedCustomer.LastName}", customer.FullName);
            Assert.AreEqual(returnedAddress.Street, customer.Street);
            Assert.AreEqual(returnedAddress.City, customer.City);
            Assert.AreEqual(returnedAddress.Postcode, customer.Postcode);
            Assert.AreEqual(returnedAddress.Country, customer.Country);

            _customerRepositoryMock.Verify();
            _addressServiceMock.Verify();
        }

        [DataTestMethod]
        [DataRow("John Doe", "John", "Doe")]
        [DataRow("Doe John", "Doe", "John")]
        [DataRow("John", "John", null)]
        public async Task GivenInsertACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsIOnSerted_AndTheCustomerIsReturned(string fullName, string firstName, string lastName)
        {
            // Arrange
            var generatedCustomerId = CreateRandomNumber();
            var addressId = CreateRandomNumber();

            var customer = new Customer
            {
                FullName = fullName,
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            _addressServiceMock.Setup(x => x.AddNewAddress(It.Is<Address>(address => 
                    address.Street == customer.Street &&
                    address.City == customer.City &&
                    address.Postcode == customer.Postcode &&
                    address.Country == customer.Country)
                )).ReturnsAsync(addressId);

            _customerRepositoryMock.Setup(x => x.GenerateIdentityAsync())
                        .ReturnsAsync(generatedCustomerId);

            _customerRepositoryMock.Setup(x => x.InsertAsync(It.Is<CustomerDto>(cust => 
                    cust.Id == generatedCustomerId &&
                    cust.FirstName == firstName &&
                    cust.LastName == lastName &&
                    cust.AddressId == addressId
                    ))).Returns(Task.CompletedTask);

            // Act
            var actualCustomerId = await CreateCustomerService().AddNewCustomer(customer);

            // Assert
            Assert.AreEqual(generatedCustomerId, actualCustomerId);

            _customerRepositoryMock.Verify();
            _addressServiceMock.Verify();
        }

        public CustomerService CreateCustomerService()
        {
            return new CustomerService(_customerRepositoryMock.Object, _addressServiceMock.Object);
        }

        private int CreateRandomNumber()
        {
            return _rnd.Next(1, int.MaxValue);
        }
    }
}