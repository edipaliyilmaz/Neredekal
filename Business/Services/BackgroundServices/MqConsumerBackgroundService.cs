using Core.Utilities.MessageBrokers;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DataAccess.Abstract;
using Business.Handlers.Reports.Commands;
using System.Text.Json;
using Entities.Concrete;
using MediatR;
using Core.Enums;
using Entities.Dtos;
using Business.Handlers.Reports.Queries;
using Business.Handlers.Hotels.Queries;
using Nest;
using Serilog.Core;
using Serilog;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Logging;
namespace Business.Services.BackgroundServices
{
    public class MqConsumerBackgroundService : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IReportRepository _reportRepository;
        private readonly IMediator _mediator;
        private readonly IMessageBrokerHelper _messageBrokerHelper;

        public MqConsumerBackgroundService(
            IMessageBrokerHelper messageBrokerHelper,
            IMessageConsumer messageConsumer,
            IReportRepository reportRepository,
            IMediator mediator)
        {
            _messageConsumer = messageConsumer;
            _reportRepository = reportRepository;
            _mediator = mediator;
            _messageBrokerHelper = messageBrokerHelper;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeMessagesAsync(), stoppingToken);
        }
        private async Task ConsumeMessagesAsync()
        {
            var logger = new LogstashLogger();

            try
            {
                var result = await _messageConsumer.GetQueue();
                if (result != null)
                {
                    var jsonResult = JsonSerializer.Deserialize<Report>(result);
                    var report = await GenerateReport();
                    if (report != null)
                    {
                        var command = new UpdateReportCommand
                        {
                            Status = ReportStatus.Completed,
                            Id = jsonResult.Id,
                        };

                        var brokerResult = await _messageBrokerHelper.QueueMessageAsync(report);
                        if (brokerResult.Success)
                        {
                            await _mediator.Send(command);
                            logger.Info("Rapor başarıyla kuyruğa gönderildi.");

                        }
                        else
                        {
                            logger.Error("Rapor kuyruğa gönderilirken bir hata oluştu.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public async Task<List<ReportItem>> GenerateReport()
        {
            var hotels = await _mediator.Send(new GetHotelsQuery { });

            var report = hotels.Data.SelectMany
                (h => h.Contacts
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

            return report;
        }
    }
}
