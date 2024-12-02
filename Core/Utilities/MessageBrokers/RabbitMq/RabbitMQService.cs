using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Core.Utilities.MessageBrokers.RabbitMq
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly MessageBrokerOptions _brokerOptions;

        public IConnection Connection => _connection;

        public RabbitMQService(IConfiguration configuration)
        {
            _brokerOptions = configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();

            var factory = new ConnectionFactory
            {
                HostName = _brokerOptions.HostName,
                UserName = _brokerOptions.UserName,
                Password = _brokerOptions.Password,
                Port = _brokerOptions.Port,
            };

            _connection = factory.CreateConnection();
        }

        public IModel CreateChannel()
        {
            return _connection.CreateModel();
        }

        public void DeclareQueue(IModel channel, string queueName)
        {
            channel.QueueDeclare(
                queue: queueName,
                durable: false, 
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
