using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.MessageBrokers
{
    public interface IRabbitMQService
    {
        IConnection Connection { get; }
        IModel CreateChannel();
        void DeclareQueue(IModel channel, string queueName);
    }
}
