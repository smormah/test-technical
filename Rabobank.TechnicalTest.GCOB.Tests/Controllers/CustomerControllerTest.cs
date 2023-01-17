using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabobank.TechnicalTest.GCOB.Controllers;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Exceptions;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerControllerTest
    {

        private Mock<ICustomerService> _customersServiceMock;
        private Mock<ILogger<CustomerController>> _loggerMock;
        private readonly Random _rnd = new Random();


        [TestInitialize]
        public void Initialize()
        {
            _customersServiceMock = new Mock<ICustomerService>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<CustomerController>>(MockBehavior.Strict);
        }


        [TestMethod]
        public async Task GivenHaveACustomer_AndIInvokeTheControllerToGetTheCustomer_ThenTheCustomerIsReturned()
        {
            // Arrange
            int identifier = CreateRandomNumber();
            var returnedCustomer = new Customer { Id = identifier };

            _customersServiceMock.Setup(x => x.GetCustomerAsync(identifier)).ReturnsAsync(returnedCustomer);

            // Act
            var response = await CreateController().Get(identifier);

            // Assert
            Assert.IsTrue(response is OkObjectResult);

            var actualCustomers = (Customer)((OkObjectResult)response).Value;
            Assert.AreEqual(returnedCustomer.Id, actualCustomers.Id);

            _customersServiceMock.Verify();
        }



        [TestMethod]
        public async Task GivenACustomerNotExistingWithAGivenIdentity_WhenIInvokeTheControllerToGetTheCustomer_ThenTheResponseIsNotFound()
        {
            // Arrange
            int identity = CreateRandomNumber();
            _customersServiceMock.Setup(x => x.GetCustomerAsync(identity)).Throws<NotFoundException>();
            _loggerMock.Setup(logger => logger.Log<It.IsAnyType>(LogLevel.Error, 0, It.Is<It.IsAnyType>((@object, type) => type.Name == "FormattedLogValues"), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            // Act
            var response = await CreateController().Get(identity);

            // Assert
            Assert.IsTrue(response is NotFoundObjectResult);

            _customersServiceMock.Verify();
            _loggerMock.Verify();
        }

        [TestMethod]
        public async Task GivenAnRaisedException_WhenIInvokeTheControllerToGetTheCustomer_ThenFailedResponseIsReturned()
        {
            // Arrange
            int identity = CreateRandomNumber();
            _customersServiceMock.Setup(x => x.GetCustomerAsync(identity)).Throws<NullReferenceException>();
            _loggerMock.Setup(logger => logger.Log<It.IsAnyType>(LogLevel.Error, 0, It.Is<It.IsAnyType>((@object, type) => type.Name == "FormattedLogValues"), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            // Act
            var response = await CreateController().Get(identity);

            // Assert
            Assert.IsFalse(response is OkObjectResult);

            var objectResult = (ObjectResult)response;
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.InternalServerError);

            _customersServiceMock.Verify();
            _loggerMock.Verify();
        }

        [TestMethod]
        public async Task GivenHaveAValidCustomer_AndIPostTheController_ThenRespondCreated()
        {
            // Arrange
            int identity = CreateRandomNumber();
            var customer = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            _customersServiceMock.Setup(x => x.AddNewCustomer(customer)).ReturnsAsync(identity);

            // Act
            var response = await CreateController().Post(customer);

            // Assert
            Assert.IsTrue(response is CreatedResult);

            var content = ((CreatedResult)response).Value?.ToString()!.Replace("=", ":");
            dynamic result = JsonConvert.DeserializeObject(content ?? string.Empty);
            Assert.AreEqual(identity, int.Parse(result.id.ToString()));

            _customersServiceMock.Verify();
        }

        [TestMethod]
        public async Task GivenHaveAnInvalidCustomerWithUnKnownCountry_AndIPostTheController_ThenResponseIsBadRequest()
        {
            // Arrange
            var customer = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            _loggerMock.Setup(logger => logger.Log<It.IsAnyType>(LogLevel.Error, 0, It.Is<It.IsAnyType>((@object, type) => type.Name == "FormattedLogValues"), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()));
            _customersServiceMock.Setup(x => x.AddNewCustomer(customer)).Throws<NotFoundException>();

            // Act
            var response = await CreateController().Post(customer);

            // Assert
            Assert.IsTrue(response is BadRequestObjectResult);
            _customersServiceMock.Verify();
            _loggerMock.Verify();
        }

        [TestMethod]
        public async Task GivenHaveAnInvalidCustomerWithRaisedException_AndIPostTheController_ThenResponseIsInternalServerError()
        {
            // Arrange
            var customer = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            _loggerMock.Setup(logger => logger.Log<It.IsAnyType>(LogLevel.Error, 0, It.Is<It.IsAnyType>((@object, type) => type.Name == "FormattedLogValues"), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()));
            _customersServiceMock.Setup(x => x.AddNewCustomer(customer)).Throws<NullReferenceException>();

            // Act
            var response = await CreateController().Post(customer);

            // Assert
            var objectResult = (ObjectResult)response;
            Assert.AreEqual(objectResult.StatusCode, (int)HttpStatusCode.InternalServerError);
            
            _customersServiceMock.Verify();
            _loggerMock.Verify();
        }

        public CustomerController CreateController()
        {
            return new CustomerController(_customersServiceMock.Object, _loggerMock.Object);
        }

        private int CreateRandomNumber()
        {
            return _rnd.Next(1, int.MaxValue);
        }     
    }
}