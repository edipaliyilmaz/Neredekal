
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
        }
    }
    public class UpdateReportValidator : AbstractValidator<UpdateReportCommand>
    {
        public UpdateReportValidator()
        {
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.CreatedDate).NotEmpty();
        }
    }
}