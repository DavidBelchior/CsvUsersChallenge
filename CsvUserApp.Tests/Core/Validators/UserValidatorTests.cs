using CsvUserApp.Core.Entities;
using CsvUserApp.Core.Validators;
using FluentValidation.TestHelper;

namespace CsvUserApp.Tests.Core.Validators
{
    public class UserValidatorTests
    {
        private readonly UserValidator _validator;

        public UserValidatorTests()
        {
            _validator = new UserValidator();
        }

        [Fact]
        public void Should_Pass_When_User_IsValid()
        {
            var user = new User(1, "Alice", "alice@test.com", 25);

            var result = _validator.TestValidate(user);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_Id_IsInvalid(int invalidId)
        {
            var user = new User(invalidId, "Alice", "alice@test.com", 25);

            var result = _validator.TestValidate(user);

            result.ShouldHaveValidationErrorFor(u => u.Id)
                  .WithErrorMessage("Id must be greater than zero");
        }

        [Fact]
        public void Should_Fail_When_Name_IsEmpty()
        {
            var user = new User(1, "", "alice@test.com", 25);

            var result = _validator.TestValidate(user);

            result.ShouldHaveValidationErrorFor(u => u.Name)
                  .WithErrorMessage("Name cannot be empty");
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Should_Fail_When_Email_IsInvalid(string invalidEmail)
        {
            var user = new User(1, "Alice", invalidEmail, 25);

            var result = _validator.TestValidate(user);

            result.ShouldHaveValidationErrorFor(u => u.Email)
                  .WithErrorMessage("Email is invalid");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(121)]
        public void Should_Fail_When_Age_IsOutOfRange(int invalidAge)
        {
            var user = new User(1, "Alice", "alice@test.com", invalidAge);

            var result = _validator.TestValidate(user);

            result.ShouldHaveValidationErrorFor(u => u.Age)
                  .WithErrorMessage("Age must be between 0 and 120");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(120)]
        public void Should_Pass_When_Age_IsOnBoundary(int validAge)
        {
            var user = new User(1, "Alice", "alice@test.com", validAge);

            var result = _validator.TestValidate(user);

            result.ShouldNotHaveAnyValidationErrors();
        }


    }
}