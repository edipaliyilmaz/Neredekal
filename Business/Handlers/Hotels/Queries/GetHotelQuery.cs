
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;


namespace Business.Handlers.Hotels.Queries
{
    public class GetHotelQuery : IRequest<IDataResult<Hotel>>
    {
        public System.Guid Id { get; set; }

        public class GetHotelQueryHandler : IRequestHandler<GetHotelQuery, IDataResult<Hotel>>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public GetHotelQueryHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Hotel>> Handle(GetHotelQuery request, CancellationToken cancellationToken)
            {
                var hotel = await _hotelRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Hotel>(hotel);
            }
        }
    }
}
