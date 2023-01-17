using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerRepositoryTest
    {
        private Mock<ILogger> _loggerMock;
        private InMemoryCustomerRepository _customerRepository;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger>();
            _customerRepository = new InMemoryCustomerRepository(_loggerMock.Object);
        }


        [TestMethod]
        public async Task GivenHaveACustomer_AndIGetTheCustomerFromTheDB_ThenTheCustomerIsRetrieved()
        {
            // Arrange
            var customerId = await SaveNewCustomer("John", "Doe");
            
            // Act
            var customer = await _customerRepository.GetAsync(customerId);

            // Assert
            Assert.AreEqual(customerId, customer.Id);
        }

        private async Task<int> SaveNewCustomer(string firstName, string lastName)
        {
            var customerId = await _customerRepository.GenerateIdentityAsync();
            var customer = new CustomerDto
            {
                Id = customerId,
                FirstName = firstName,
                LastName = lastName,
                AddressId = 1
            };

            await _customerRepository.InsertAsync(customer);
            return customerId;
        }
    }
}
