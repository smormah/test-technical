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
    public class AddressServiceTest
    {
        private Mock<IAddressRepository> _addressRepositoryMock;
        private Mock<ICountryService> _countryServiceMock;
        private readonly Random _rnd = new Random();

        [TestInitialize]
        public void Initialize()
        {
            _addressRepositoryMock = new Mock<IAddressRepository>(MockBehavior.Strict);
            _countryServiceMock = new Mock<ICountryService>(MockBehavior.Strict);
        }


        [TestMethod]
        public async Task GivenHaveAnAddress_AndICallAServiceToGetTheAddress_ThenTheAddressIsReturned()
        {
            // Arrange
            var addressId = CreateRandomNumber();

            var returnedCountry = new Country
            {
                Id = CreateRandomNumber(),
                Name = "UK"
            };

            var returnedAddress = new AddressDto
            {
                Id = addressId,
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                CountryId = returnedCountry.Id
            };



            _addressRepositoryMock.Setup(x => x.GetAsync(addressId)).ReturnsAsync(returnedAddress);
            _countryServiceMock.Setup(x => x.GetCountryAsync(returnedCountry.Id)).ReturnsAsync(returnedCountry);

            // Act
            var address = await CreateAddressService().GetAddressAsync(addressId);

            // Assert
            Assert.IsNotNull(address);;
            Assert.AreEqual(returnedAddress.Id, address.Id);
            Assert.AreEqual(returnedAddress.Street, address.Street);
            Assert.AreEqual(returnedAddress.City, address.City);
            Assert.AreEqual(returnedAddress.Postcode, address.Postcode);
            Assert.AreEqual(returnedCountry.Name, address.Country);

            _addressRepositoryMock.Verify();
            _countryServiceMock.Verify();
        }

        [TestMethod]
        public async Task GivenInsertAAddress_AndICallAServiceToGetTheAddress_ThenTheAddressIsIOnSerted_AndTheAddressIsReturned()
        {
            // Arrange
            var addressId = CreateRandomNumber();
            var countryId = CreateRandomNumber();

            var address = new Address
            {
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };
            
            _addressRepositoryMock.Setup(x => x.GenerateIdentityAsync()).ReturnsAsync(addressId);

            _addressRepositoryMock.Setup(x => x.InsertAsync(It.Is<AddressDto>(addr =>
                        addr.Id == addressId &&
                        addr.Street == address.Street &&
                        addr.City == address.City &&
                        addr.Postcode == address.Postcode &&
                        addr.CountryId == countryId
                    ))).Returns(Task.CompletedTask);

            _countryServiceMock.Setup(x => x.GetCountryId(address.Country)).ReturnsAsync(countryId);

            // Act
            var actualAddressId = await CreateAddressService().AddNewAddress(address);

            // Assert
            Assert.IsNotNull(address); ;
            Assert.AreEqual(addressId, actualAddressId);

            _addressRepositoryMock.Verify();
            _countryServiceMock.Verify();
        }

        public AddressService CreateAddressService()
        {
            return new AddressService(_addressRepositoryMock.Object, _countryServiceMock.Object);
        }

        private int CreateRandomNumber()
        {
            return _rnd.Next(1, int.MaxValue);
        }
    }
}