using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.Reports.Commands;
using Business.Handlers.Reports.Queries;
using MediatR;
using Core.Enums;
using Core.Utilities.MessageBrokers;
using System.Collections.Generic;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Logging;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Business.Handlers.Hotels.Queries;
using System.Linq;

namespace Business.Services.BackgroundServices
{
    [LogAspect(typeof(LogstashLogger))]
    public class MqConsumerBackgroundService : BackgroundService
    {
        private readonly IMessageBrokerHelper _messageBrokerHelper;
        private readonly IReportRepository _reportRepository;
        private readonly IMediator _mediator;
        private readonly IRabbitMQService _rabbitMQService;

        public MqConsumerBackgroundService(
            IMessageBrokerHelper messageBrokerHelper,
            IReportRepository reportRepository,
            IMediator mediator,
            IRabbitMQService rabbitMQService)
        {
            _messageBrokerHelper = messageBrokerHelper;
            _reportRepository = reportRepository;
            _mediator = mediator;
            _rabbitMQService = rabbitMQService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => StartConsuming(stoppingToken), stoppingToken);
            return Task.CompletedTask;
        }

        private void StartConsuming(CancellationToken stoppingToken)
        {
            try
            {
                var channel = _rabbitMQService.CreateChannel();
                var queueName = "Report";

                _rabbitMQService.DeclareQueue(channel, queueName);

                var consumer = new EventingBasicConsumer(channel);
                Console.WriteLine("Message received: consumer");

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"Message received: {message}");

                        var report = JsonSerializer.Deserialize<Report>(message);
                        if (report != null)
                        {
                            var reports = await _mediator.Send(new GetHotelsWithContactQuery());
                            if (reports.Data != null)
                            {

                                var command = new UpdateReportCommand
                                {
                                    Status = ReportStatus.Completed,
                                    Id = report.Id
                                };

                                var result = await _mediator.Send(command);
                                var messageQueueResult = await _messageBrokerHelper.QueueMessageAsync(reports.Data,"ReportResult");
                                Console.WriteLine($"Report processed: {result}");
                                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                };

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while consuming: {ex.Message}");
            }
        }
    }
}
