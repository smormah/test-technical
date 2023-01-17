using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CountryServiceTest
    {
        private Mock<ICountryRepository> _countryRepositoryMock;
        private readonly Random _rnd = new Random();

        [TestInitialize]
        public void Initialize()
        {
            _countryRepositoryMock = new Mock<ICountryRepository>(MockBehavior.Strict);
        }


        [TestMethod]
        public async Task GivenHaveACountry_AndICallAServiceToGetTheCountry_ThenTheCountryIsReturned()
        {
            // Arrange
            var countryId = CreateRandomNumber();

            var returnedCountry = new CountryDto
            {
                Id = CreateRandomNumber(),
                Name = "UK"
            };

            _countryRepositoryMock.Setup(x => x.GetAsync(countryId)).ReturnsAsync(returnedCountry);

            // Act
            var country = await CreateCountryService().GetCountryAsync(countryId);

            // Assert
            Assert.IsNotNull(country); ;
            Assert.AreEqual(returnedCountry.Id, country.Id);
            Assert.AreEqual(returnedCountry.Name, country.Name);

            _countryRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task GivenACountryNameIsProvided_AndICallTheService_ThenTheCountryIdIsReturned()
        {
            const string countryName = "UK";

            var returnedCountries = new List<CountryDto>
            {
                new CountryDto { Id = 1, Name = "Ireland" },
                new CountryDto { Id = 2, Name = "UK" }
            };

            _countryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(returnedCountries);

            // Act
            var actualAddressId = await CreateCountryService().GetCountryId(countryName);

            // Assert
            var expectedCountryId = returnedCountries.Single(x => x.Name == countryName).Id;
            Assert.AreEqual(expectedCountryId, actualAddressId);

            _countryRepositoryMock.Verify();
        }

        public CountryService CreateCountryService()
        {
            return new CountryService(_countryRepositoryMock.Object);
        }

        private int CreateRandomNumber()
        {
            return _rnd.Next(1, int.MaxValue);
        }
    }
}