
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
using Microsoft.EntityFrameworkCore;
using Entities.Dtos;
using System.Linq;
using Core.Enums;

namespace Business.Handlers.Hotels.Queries
{

    public class GetHotelsInfosQuery : IRequest<IDataResult<IEnumerable<HotelDto>>>
    {
        public class GetHotelsInfosQueryHandler : IRequestHandler<GetHotelsInfosQuery, IDataResult<IEnumerable<HotelDto>>>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public GetHotelsInfosQueryHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<HotelDto>>> Handle(GetHotelsInfosQuery request, CancellationToken cancellationToken)
            {
                var hotels = await _hotelRepository.GetHotelWithContactAsync().ToListAsync(cancellationToken);
                var hotelDto = hotels.Select(hotel => new HotelDto
                {
                    CompanyName = hotel.CompanyName,
                    ManagerFirstName = hotel.ManagerFirstName,
                    ManagerLastName = hotel.ManagerLastName,
                    Contacts = hotel.Contacts.Select(contact => new ContactDto
                    {
                        Type = contact.Type,
                        Value = contact.Value
                    }).ToList(),
                });

                return new SuccessDataResult<IEnumerable<HotelDto>>(hotelDto);
            }

        }
    }
}