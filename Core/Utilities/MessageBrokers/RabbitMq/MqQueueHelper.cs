using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Core.Utilities.MessageBrokers.RabbitMq
{
    public class RMqQueueHelper : IMessageBrokerHelper
    {
        private readonly MessageBrokerOptions _brokerOptions;
        private readonly IRabbitMQService _rabbitMQService;

        public RMqQueueHelper(IConfiguration configuration, IRabbitMQService rabbitMQService)
        {
            Configuration = configuration;
            _brokerOptions = Configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();
            _rabbitMQService = rabbitMQService;
        }

        public IConfiguration Configuration { get; }

        public Task<IResult> QueueMessageAsync<T>(T messageModel)
        {
            var channel = _rabbitMQService.CreateChannel();
            var queueName = "Report"; 

            _rabbitMQService.DeclareQueue(channel, queueName);

            var message = JsonConvert.SerializeObject(messageModel);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);

            return Task.FromResult<IResult>(new SuccessResult());
        }
    }
}
