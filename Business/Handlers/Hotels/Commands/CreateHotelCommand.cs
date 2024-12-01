
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
using Business.Handlers.Hotels.ValidationRules;
using Entities.Dtos;
using System.Collections.Generic;
using System;
using Business.Helpers;

namespace Business.Handlers.Hotels.Commands
{

    /// <summary>
    /// 
    /// </summary>
    public class CreateHotelCommand : IRequest<IResult>
    {

        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string CompanyName { get; set; }
        public ICollection<ContactDto> Contacts { get; set; }


        public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, IResult>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;
            public CreateHotelCommandHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateHotelValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
            {
                var isThereHotelRecord = _hotelRepository.Query().Any(u => u.CompanyName == request.CompanyName);

                if (isThereHotelRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var newHotel = new Hotel
                {
                    Id = SequentialGuidGenerator.NewSequentialGuid(),  
                    ManagerFirstName = request.ManagerFirstName,
                    ManagerLastName = request.ManagerLastName,
                    CompanyName = request.CompanyName
                };

                if (request.Contacts != null && request.Contacts.Any())
                {
                    newHotel.Contacts = request.Contacts.Select(c => new Contact
                    {
                        Id = SequentialGuidGenerator.NewSequentialGuid(),
                        HotelId = newHotel.Id,  
                        Type = c.Type,
                        Value = c.Value
                    }).ToList();
                }

                _hotelRepository.Add(newHotel);

                await _hotelRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Added);
            }

        }
    }
}