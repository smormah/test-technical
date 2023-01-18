using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabobank.TechnicalTest.GCOB.Domain;
using Rabobank.TechnicalTest.GCOB.Validators;

namespace Rabobank.TechnicalTest.GCOB.Tests.Validators
{
    [TestClass]
    public class CustomerValidatorTest
    {
        [TestMethod]
        public async Task CustomerValidator_ShouldPass_WhenValid()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task CustomerValidator_ShouldFail_WhenWithMissingFullName()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task CustomerValidator_ShouldFail_WhenWithMissingStreet()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Country = "UK"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task CustomerValidator_ShouldFail_WhenWithMissingPostcode()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                FullName = "John Doe",
                City = "London",
                Street = "123 Fake Street",
                Country = "UK"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task CustomerValidator_ShouldFail_WhenWithMissingCity()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                Street = "123 Fake Street",
                Country = "UK"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task CustomerValidator_ShouldFail_WhenWithMissingCountry()
        {
            var validator = new CustomerValidator();

            var dto = new Customer
            {
                FullName = "John Doe",
                Postcode = "XXX A1B",
                City = "London",
                Street = "123 Fake Street"
            };

            var result = await validator.TestValidateAsync(dto);

            Assert.IsFalse(result.IsValid);
        }
    }
}
