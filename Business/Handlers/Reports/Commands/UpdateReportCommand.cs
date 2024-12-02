
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Reports.ValidationRules;


namespace Business.Handlers.Reports.Commands
{


    public class UpdateReportCommand : IRequest<IResult>
    {
        public System.Guid Id { get; set; }
        public Core.Enums.ReportStatus Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int PhoneCount { get; set; }

        public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, IResult>
        {
            private readonly IReportRepository _reportRepository;
            private readonly IMediator _mediator;

            public UpdateReportCommandHandler(IReportRepository reportRepository, IMediator mediator)
            {
                _reportRepository = reportRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateReportValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
            {
                var isThereReportRecord = await _reportRepository.GetAsync(u => u.Id == request.Id);

                isThereReportRecord.Status = request.Status;


                _reportRepository.Update(isThereReportRecord);
                await _reportRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

