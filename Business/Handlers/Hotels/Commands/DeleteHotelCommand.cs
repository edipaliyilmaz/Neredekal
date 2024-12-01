
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Business.Handlers.Hotels.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteHotelCommand : IRequest<IResult>
    {
        public System.Guid Id { get; set; }

        public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand, IResult>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public DeleteHotelCommandHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
            {
                var hotelToDelete = _hotelRepository.Get(p => p.Id == request.Id);

                _hotelRepository.Delete(hotelToDelete);
                await _hotelRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

