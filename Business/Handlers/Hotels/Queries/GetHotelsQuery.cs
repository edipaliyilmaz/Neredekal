
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

namespace Business.Handlers.Hotels.Queries
{

    public class GetHotelsQuery : IRequest<IDataResult<IEnumerable<Hotel>>>
    {
        public class GetHotelsQueryHandler : IRequestHandler<GetHotelsQuery, IDataResult<IEnumerable<Hotel>>>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public GetHotelsQueryHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<Hotel>>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Hotel>>(await _hotelRepository.GetListAsync());
            }
        }
    }
}