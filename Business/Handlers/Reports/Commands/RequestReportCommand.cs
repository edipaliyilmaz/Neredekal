
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
using Microsoft.AspNetCore.Mvc;

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
                    CreateDate = DateTime.Now,
                };

                var messageQueueResult = await _messageBrokerHelper.QueueMessageAsync(reportRequest, "Report");
                if (messageQueueResult.Success)
                {
                    var createReportCommand = new CreateReportCommand
                    {
                        Id = reportRequest.Id,
                        Status = reportRequest.Status,
                        CreatedDate = reportRequest.CreateDate,
                    };

                    await _mediator.Send(createReportCommand, cancellationToken);

                    Console.WriteLine("Report message queued and command dispatched successfully.");
                }
                else
                {
                    Console.WriteLine($"Error queuing report message: {messageQueueResult.Message}");
                    return new ErrorResult("QueueingFailed");
                }

                return new SuccessResult(Messages.Added);
            }

        }
    }
}