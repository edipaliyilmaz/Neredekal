
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


namespace Business.Handlers.ContactInfoes.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteContactInfoCommand : IRequest<IResult>
    {
        public System.Guid Id { get; set; }

        public class DeleteContactInfoCommandHandler : IRequestHandler<DeleteContactInfoCommand, IResult>
        {
            private readonly IContactInfoRepository _contactInfoRepository;
            private readonly IMediator _mediator;

            public DeleteContactInfoCommandHandler(IContactInfoRepository contactInfoRepository, IMediator mediator)
            {
                _contactInfoRepository = contactInfoRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteContactInfoCommand request, CancellationToken cancellationToken)
            {
                var contactInfoToDelete = _contactInfoRepository.Get(p => p.Id == request.Id);

                _contactInfoRepository.Delete(contactInfoToDelete);
                await _contactInfoRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

