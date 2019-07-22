namespace Infrastructure.Models.Validators
{
    using FluentValidation;
    using Infrastructure.Models.Models;
    using System;
    using System.Linq;
    public class UserValidator : AbstractValidator<User>
    {
        private readonly char[] forbiddenCharacters = "!@#$%^&*(){}[]_-+=~`,/?|\\;'".ToCharArray();
        private readonly DateTime minDate = new DateTime(2019, 6, 28, 20, 0, 0, 0);
        public UserValidator()
        {
            RuleFor(u => u.Username)
                  .NotEmpty()
                  .NotNull()
                  .MinimumLength(6)
                  .MaximumLength(32)
                  .Must(x => x.All(s => !forbiddenCharacters.Contains(s)))
                  .WithMessage("UserName contains unallowed symbols!");

            RuleFor(r => r.RegisteredOn)
            .Must(r=>r<DateTime.UtcNow&&r>minDate)
            .WithMessage("Time of registration is fake!");

              RuleFor(u => u.Password)
                    .NotEmpty()
                    .NotNull()
                    .MinimumLength(6)
                    .MaximumLength(32)
                    .Must(PasswordIsValid)
                    .WithMessage("Password must have atleast one: lowercase/uppercase letter and digit also must be between 6 and 32 signs long!");
        }

      private bool PasswordIsValid(string password)
      {
          bool containsUpperCase = password.Any(x => char.IsUpper(x));
          bool containsLowerCase = password.Any(x => char.IsLower(x));
          bool containsDigit = password.Any(x => char.IsDigit(x));
          return containsUpperCase && containsLowerCase && containsDigit;
      }
    }
}