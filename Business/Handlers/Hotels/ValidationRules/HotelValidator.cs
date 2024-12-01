
using Business.Handlers.Hotels.Commands;
using FluentValidation;

namespace Business.Handlers.Hotels.ValidationRules
{

    public class CreateHotelValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelValidator()
        {
            RuleFor(x => x.ManagerFirstName).NotEmpty();
            RuleFor(x => x.ManagerLastName).NotEmpty();
            RuleFor(x => x.CompanyName).NotEmpty();
            RuleFor(x => x.Contacts).NotEmpty();

        }
    }
    public class UpdateHotelValidator : AbstractValidator<UpdateHotelCommand>
    {
        public UpdateHotelValidator()
        {
            RuleFor(x => x.ManagerFirstName).NotEmpty();
            RuleFor(x => x.ManagerLastName).NotEmpty();
            RuleFor(x => x.CompanyName).NotEmpty();
            RuleFor(x => x.Contacts).NotEmpty();

        }
    }
}