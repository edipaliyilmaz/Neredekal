
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
using Business.Handlers.ContactInfoes.ValidationRules;


namespace Business.Handlers.ContactInfoes.Commands
{


    public class UpdateContactInfoCommand : IRequest<IResult>
    {
        public System.Guid Id { get; set; }
        public Core.Enums.ContactType Type { get; set; }
        public string Value { get; set; }

        public class UpdateContactInfoCommandHandler : IRequestHandler<UpdateContactInfoCommand, IResult>
        {
            private readonly IContactInfoRepository _contactInfoRepository;
            private readonly IMediator _mediator;

            public UpdateContactInfoCommandHandler(IContactInfoRepository contactInfoRepository, IMediator mediator)
            {
                _contactInfoRepository = contactInfoRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateContactInfoValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(UpdateContactInfoCommand request, CancellationToken cancellationToken)
            {
                var isThereContactRecord = await _contactInfoRepository.GetAsync(u => u.Id == request.Id);

                isThereContactRecord.Type = request.Type;
                isThereContactRecord.Value = request.Value;

                _contactInfoRepository.Update(isThereContactRecord);
                await _contactInfoRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

