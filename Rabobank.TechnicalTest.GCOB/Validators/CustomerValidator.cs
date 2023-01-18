using FluentValidation;
using Rabobank.TechnicalTest.GCOB.Domain;

namespace Rabobank.TechnicalTest.GCOB.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FullName).NotNull().NotEmpty().WithMessage("Full name is required");
            RuleFor(x => x.Street).NotNull().NotEmpty().WithMessage("Street name is required");
            RuleFor(x => x.City).NotNull().NotEmpty().WithMessage("City is required");
            RuleFor(x => x.Postcode).NotNull().NotEmpty().WithMessage("Postcode is required");
            RuleFor(x => x.Country).NotNull().NotEmpty().WithMessage("Country is required");
        }
    }
}
