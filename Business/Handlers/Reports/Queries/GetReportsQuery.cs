
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;

namespace Business.Handlers.Reports.Queries
{

    public class GetReportsQuery : IRequest<IDataResult<IEnumerable<Report>>>
    {
        public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, IDataResult<IEnumerable<Report>>>
        {
            private readonly IReportRepository _reportRepository;
            private readonly IMediator _mediator;

            public GetReportsQueryHandler(IReportRepository reportRepository, IMediator mediator)
            {
                _reportRepository = reportRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<Report>>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Report>>(await _reportRepository.GetListAsync());
            }
        }
    }
}