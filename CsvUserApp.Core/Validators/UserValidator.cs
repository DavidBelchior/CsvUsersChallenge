using CsvUserApp.Core.Entities;
using FluentValidation;

namespace CsvUserApp.Core.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Id).GreaterThan(0).WithMessage("Id must be greater than zero");
            RuleFor(u => u.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("Email is invalid");
            RuleFor(u => u.Age).InclusiveBetween(0, 120).WithMessage("Age must be between 0 and 120");
        }
    }
}
