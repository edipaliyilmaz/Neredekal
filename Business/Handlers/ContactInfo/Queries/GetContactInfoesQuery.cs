
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

namespace Business.Handlers.ContactInfoes.Queries
{

    public class GetContactInfoesQuery : IRequest<IDataResult<IEnumerable<Contact>>>
    {
        public class GetContactInfoesQueryHandler : IRequestHandler<GetContactInfoesQuery, IDataResult<IEnumerable<Contact>>>
        {
            private readonly IContactInfoRepository _contactInfoRepository;
            private readonly IMediator _mediator;

            public GetContactInfoesQueryHandler(IContactInfoRepository contactInfoRepository, IMediator mediator)
            {
                _contactInfoRepository = contactInfoRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<IEnumerable<Contact>>> Handle(GetContactInfoesQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Contact>>(await _contactInfoRepository.GetListAsync());
            }
        }
    }
}