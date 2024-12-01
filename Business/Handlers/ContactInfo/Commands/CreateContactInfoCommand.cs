
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
using Business.Handlers.ContactInfoes.ValidationRules;

namespace Business.Handlers.ContactInfoes.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateContactInfoCommand : IRequest<IResult>
    {

        public System.Guid HotelId { get; set; }
        public Core.Enums.ContactType Type { get; set; }
        public string Value { get; set; }


        public class CreateContactInfoCommandHandler : IRequestHandler<CreateContactInfoCommand, IResult>
        {
            private readonly IContactInfoRepository _contactInfoRepository;
            private readonly IMediator _mediator;
            public CreateContactInfoCommandHandler(IContactInfoRepository contactInfoRepository, IMediator mediator)
            {
                _contactInfoRepository = contactInfoRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateContactInfoValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateContactInfoCommand request, CancellationToken cancellationToken)
            {
               
                var addedContactInfo = new Contact
                {
                    HotelId = request.HotelId,
                    Type = request.Type,
                    Value = request.Value,

                };

                _contactInfoRepository.Add(addedContactInfo);
                await _contactInfoRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}