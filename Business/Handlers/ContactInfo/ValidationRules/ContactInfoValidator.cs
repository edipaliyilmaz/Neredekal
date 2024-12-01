
using Business.Handlers.ContactInfoes.Commands;
using FluentValidation;

namespace Business.Handlers.ContactInfoes.ValidationRules
{

    public class CreateContactInfoValidator : AbstractValidator<CreateContactInfoCommand>
    {
        public CreateContactInfoValidator()
        {
            RuleFor(x => x.HotelId).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();

        }
    }
    public class UpdateContactInfoValidator : AbstractValidator<UpdateContactInfoCommand>
    {
        public UpdateContactInfoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();

        }
    }
}