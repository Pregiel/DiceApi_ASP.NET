using DiceApi.Dtos;
using FluentValidation;

namespace DiceApi.Validators
{
    public class RoomValidator : AbstractValidator<RoomDto>
    {
        public RoomValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage(Properties.resultMessages.TitleNull)
                .Length(6, 32).WithMessage(Properties.resultMessages.TitleLength);
            RuleFor(x => x.Password)
                .NotNull().WithMessage(Properties.resultMessages.PasswordNull)
                .Length(6, 32).WithMessage(Properties.resultMessages.PasswordLength);
        }
    }
}
