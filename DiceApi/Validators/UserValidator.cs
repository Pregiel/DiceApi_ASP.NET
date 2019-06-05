using DiceApi.Dtos;
using FluentValidation;

namespace DiceApi.Validators
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Username)
                .NotNull().WithMessage(Properties.resultMessages.UsernameNull)
                .Length(4, 32).WithMessage(Properties.resultMessages.UsernameLength);
            RuleFor(x => x.Password)
                .NotNull().WithMessage(Properties.resultMessages.PasswordNull)
                .Length(6, 32).WithMessage(Properties.resultMessages.PasswordLength);
        }
    }
}
