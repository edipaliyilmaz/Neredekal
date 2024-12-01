
using Business.Handlers.Reports.Commands;
using FluentValidation;

namespace Business.Handlers.Reports.ValidationRules
{

    public class CreateReportValidator : AbstractValidator<CreateReportCommand>
    {
        public CreateReportValidator()
        {
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.CreatedDate).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
            RuleFor(x => x.HotelCount).NotEmpty();
            RuleFor(x => x.PhoneCount).NotEmpty();

        }
    }
    public class UpdateReportValidator : AbstractValidator<UpdateReportCommand>
    {
        public UpdateReportValidator()
        {
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.CreatedDate).NotEmpty();
            RuleFor(x => x.Location).NotEmpty();
            RuleFor(x => x.HotelCount).NotEmpty();
            RuleFor(x => x.PhoneCount).NotEmpty();

        }
    }
}