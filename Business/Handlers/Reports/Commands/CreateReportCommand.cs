
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
using Business.Helpers;
using Core.Enums;
using System;

namespace Business.Handlers.Reports.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateReportCommand : IRequest<IResult>
    {
        public ReportStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid Id { get; set; }

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
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateReportCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var addedReport = new Report
                    {
                        Id = request.Id,
                        Status = request.Status,
                        CreateDate = request.CreatedDate,
                    };

                    _reportRepository.Add(addedReport);
                    await _reportRepository.SaveChangesAsync();
                    return new SuccessResult(Messages.Added);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}