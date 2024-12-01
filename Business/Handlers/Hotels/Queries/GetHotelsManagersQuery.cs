
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
using Entities.Dtos;
using System.Linq;

namespace Business.Handlers.Hotels.Queries
{

    public class GetHotelsManagersQuery : IRequest<IDataResult<IEnumerable<ManagerDto>>>
    {
        public class GetHotelsManagersQueryHandler : IRequestHandler<GetHotelsManagersQuery, IDataResult<IEnumerable<ManagerDto>>>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public GetHotelsManagersQueryHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<ManagerDto>>> Handle(GetHotelsManagersQuery request, CancellationToken cancellationToken)
            {
                var result = await _hotelRepository.GetListAsync();
                return new SuccessDataResult<IEnumerable<ManagerDto>>(result.Select(x=> new ManagerDto { ManagerFirstName = x.ManagerFirstName , ManagerLastName =x.ManagerLastName}));
            }
        }
    }
}