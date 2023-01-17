using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Rabobank.TechnicalTest.GCOB.Exceptions;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CountryRepositoryTest
    {
        private Mock<ILogger> _loggerMock;
        private InMemoryCountryRepository _countryRepository;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger>();
            _countryRepository = new InMemoryCountryRepository(_loggerMock.Object);
        }

        [DataTestMethod]
        [DataRow(1, "Netherlands")]
        [DataRow(2, "Poland")]
        [DataRow(3, "Ireland")]
        [DataRow(4, "South Africa")]
        [DataRow(5, "India")]
        public async Task GivenHaveACountry_AndIGetTheCountryFromTheDB_ThenTheCountryIsRetrieved(int countryId, string name)
        {
            
            // Act
            var country = await _countryRepository.GetAsync(countryId);

            // Assert
            Assert.AreEqual(country.Name, name);
        }

        [TestMethod]
        public async Task GivenAnInvalidCountryIdIsSupplied_WhenIGetTheCountryFromTheDB_ThenANotFoundExceptionIsThrown()
        {
            // Arrange
            const int countryId = 10000;

            // Act
            var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await _countryRepository.GetAsync(countryId));

            // Assert
            Assert.IsTrue(exception.Message == $"{countryId}");
        }
    }
}
