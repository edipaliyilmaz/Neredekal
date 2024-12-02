
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
using Business.Handlers.Reports.ValidationRules;
using Business.Helpers;
using Core.Utilities.MessageBrokers;
using ServiceStack;
using System;
using Entities.Dtos;
using Core.Enums;

namespace Business.Handlers.Reports.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestReportCommand : IRequest<IResult>
    {

        public class RequestReportCommandHandler : IRequestHandler<RequestReportCommand, IResult>
        {
            private readonly IReportRepository _reportRepository;
            private readonly IMediator _mediator;
            private readonly IMessageConsumer _messageConsumer;
            private readonly IMessageBrokerHelper _messageBrokerHelper;
            public RequestReportCommandHandler(IReportRepository reportRepository, IMediator mediator, IMessageConsumer messageConsumer, IMessageBrokerHelper messageBrokerHelper)
            {
                _reportRepository = reportRepository;
                _mediator = mediator;
                _messageConsumer = messageConsumer;
                _messageBrokerHelper = messageBrokerHelper;
            }

            [ValidationAspect(typeof(CreateReportValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(RequestReportCommand request, CancellationToken cancellationToken)
            {
                var reportId = SequentialGuidGenerator.NewSequentialGuid();


                var reportRequest = new Report
                {
                    Id = reportId,
                    Status = ReportStatus.Preparing,
                };
                var result = await _messageBrokerHelper.QueueMessageAsync(reportRequest);
                if (result.Success)
                {
                    Console.WriteLine("Message queued successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: {result.Message}");
                }
                //var addedReport = new Report
                //{
                //    Id = SequentialGuidGenerator.NewSequentialGuid(),
                //    Status = request.Status,
                //};

                //_reportRepository.Add(addedReport);
                //await _reportRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}