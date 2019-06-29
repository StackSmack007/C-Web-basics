namespace Infrastructure.Models.Validators
{
    using FluentValidation;
    using Infrastructure.Models.Models;
    using System;
    using System.Linq;
    public class ProductValidator : AbstractValidator<Product>
    {
        private readonly char[] forbiddenCharacters = "!@#$%^&*(){}[]_-+=~`,/?|\\;'".ToCharArray();
        public ProductValidator()
        {
            RuleFor(p => p.ProductName)
                  .NotEmpty()
                  .NotNull()
                  .MinimumLength(4)
                  .MaximumLength(32)
                  .Must(x => x.All(s => !forbiddenCharacters.Contains(s)))
                  .WithMessage("ProductName contains unallowed symbols!");

            RuleFor(p => p.Price)
                .Must(p => p > 0)
                .WithMessage("Price can not be less than 0");
        }
    }
}