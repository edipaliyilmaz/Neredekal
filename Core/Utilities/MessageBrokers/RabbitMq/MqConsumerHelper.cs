using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Core.Enums;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Core.Utilities.MessageBrokers.RabbitMq;

public class MqConsumerHelper : IMessageConsumer
{
    private readonly MessageBrokerOptions _brokerOptions;
    private readonly IConfiguration _configuration;

    public MqConsumerHelper(IConfiguration configuration)
    {
        _configuration = configuration;
        _brokerOptions = _configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();
    }

    public Task<string> GetQueue()
    {
        var tcs = new TaskCompletionSource<string>(); 

        try
        {
            using var connection = new ConnectionFactory()
            {
                HostName = _brokerOptions.HostName,
                Port = _brokerOptions.Port,
                UserName = _brokerOptions.UserName,
                Password = _brokerOptions.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(2000),
            }
            .CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                "Report",
                false,
                false,
                false,
                null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, mq) =>
            {
                var body = mq.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                tcs.SetResult(message);
            };

            channel.BasicConsume(
                "Report",
                true,
                consumer);

            return tcs.Task;
        }
        catch (Exception ex)
        {
            tcs.SetException(ex); 
            return tcs.Task;
        }
    }

}