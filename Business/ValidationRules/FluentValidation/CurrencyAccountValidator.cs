using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class CurrencyAccountValidator : AbstractValidator<CurrencyAccount>
    {
        public CurrencyAccountValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Cari ad boş olamaz");
            RuleFor(p => p.Name).MinimumLength(4).WithMessage("Cari ad en az 4 karakter olmalıdır");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Cari adres boş olamaz");
            RuleFor(p => p.Address).MinimumLength(4).WithMessage("Cari adres en az 4 karakter olmalıdır");
        }
    }
}