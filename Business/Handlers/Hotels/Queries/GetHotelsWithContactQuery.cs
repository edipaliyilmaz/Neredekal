
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

    public class GetHotelsWithContactQuery : IRequest<IDataResult<IEnumerable<ReportItem>>>
    {
        public class GetHotelsWithContactQueryHandler : IRequestHandler<GetHotelsWithContactQuery, IDataResult<IEnumerable<ReportItem>>>
        {
            private readonly IHotelRepository _hotelRepository;
            private readonly IMediator _mediator;

            public GetHotelsWithContactQueryHandler(IHotelRepository hotelRepository, IMediator mediator)
            {
                _hotelRepository = hotelRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<ReportItem>>> Handle(GetHotelsWithContactQuery request, CancellationToken cancellationToken)
            {
                var hotels = await _hotelRepository.GetHotelWithContactAsync().ToListAsync(cancellationToken);

                var reportItems = hotels
                    .SelectMany(h => h.Contacts
                        .Where(c => c.Type == ContactType.Location)
                        .Select(locationContact => new
                        {
                            Location = locationContact.Value,
                            Hotel = h
                        }))
                    .GroupBy(x => x.Location)
                    .Select(group => new ReportItem
                    {
                        Location = group.Key,
                        HotelCount = group.Count(),
                        PhoneCount = group.SelectMany(x => x.Hotel.Contacts)
                                          .Count(c => c.Type == ContactType.PhoneNumber)
                    })
                    .ToList();

                return new SuccessDataResult<IEnumerable<ReportItem>>(reportItems);
            }

        }
    }
}