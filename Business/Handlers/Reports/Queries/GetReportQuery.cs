
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using System;


namespace Business.Handlers.Reports.Queries
{
    public class GetReportQuery : IRequest<IDataResult<Report>>
    {
        public Guid Id { get; set; }

        public class GetReportQueryHandler : IRequestHandler<GetReportQuery, IDataResult<Report>>
        {
            private readonly IReportRepository _reportRepository;
            private readonly IMediator _mediator;

            public GetReportQueryHandler(IReportRepository reportRepository, IMediator mediator)
            {
                _reportRepository = reportRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Report>> Handle(GetReportQuery request, CancellationToken cancellationToken)
            {
                var report = await _reportRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Report>(report);
            }
        }
    }
}
