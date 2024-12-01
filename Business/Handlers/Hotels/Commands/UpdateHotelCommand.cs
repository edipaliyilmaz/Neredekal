
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
using Business.Handlers.Hotels.ValidationRules;


namespace Business.Handlers.Hotels.Commands
{


    public class UpdateHotelCommand : IRequest<IResult>
    {
        public System.Guid Id { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string CompanyName { get; set; }
        public System.Collections.Generic.ICollection<Contact> Contacts { get; set; }

        public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, IResult>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public UpdateHotelCommandHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateHotelValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
            {
                var isThereHotelRecord = await _hotelRepository.GetAsync(u => u.Id == request.Id);


                isThereHotelRecord.ManagerFirstName = request.ManagerFirstName;
                isThereHotelRecord.ManagerLastName = request.ManagerLastName;
                isThereHotelRecord.CompanyName = request.CompanyName;
                isThereHotelRecord.Contacts = request.Contacts;


                _hotelRepository.Update(isThereHotelRecord);
                await _hotelRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

