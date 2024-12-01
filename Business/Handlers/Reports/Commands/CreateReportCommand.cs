
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Reports.ValidationRules;

namespace Business.Handlers.Reports.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateReportCommand : IRequest<IResult>
    {

        public Core.Enums.ReportStatus Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int PhoneCount { get; set; }


        public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, IResult>
        {
            private readonly IReportRepository _reportRepository;
            private readonly IMediator _mediator;
            public CreateReportCommandHandler(IReportRepository reportRepository, IMediator mediator)
            {
                _reportRepository = reportRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateReportValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateReportCommand request, CancellationToken cancellationToken)
            {
                var isThereReportRecord = _reportRepository.Query().Any(u => u.Status == request.Status);

                if (isThereReportRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedReport = new Report
                {
                    Status = request.Status,
                    CreatedDate = request.CreatedDate,
                    Location = request.Location,
                    HotelCount = request.HotelCount,
                    PhoneCount = request.PhoneCount,

                };

                _reportRepository.Add(addedReport);
                await _reportRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}