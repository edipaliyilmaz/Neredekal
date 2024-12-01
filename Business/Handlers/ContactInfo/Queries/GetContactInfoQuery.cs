
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;


namespace Business.Handlers.ContactInfoes.Queries
{
    public class GetContactInfoQuery : IRequest<IDataResult<Contact>>
    {
        public System.Guid Id { get; set; }

        public class GetContactInfoQueryHandler : IRequestHandler<GetContactInfoQuery, IDataResult<Contact>>
        {
            private readonly IContactInfoRepository _contactInfoRepository;
            private readonly IMediator _mediator;

            public GetContactInfoQueryHandler(IContactInfoRepository contactInfoRepository, IMediator mediator)
            {
                _contactInfoRepository = contactInfoRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(LogstashLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Contact>> Handle(GetContactInfoQuery request, CancellationToken cancellationToken)
            {
                var contactInfo = await _contactInfoRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Contact>(contactInfo);
            }
        }
    }
}
