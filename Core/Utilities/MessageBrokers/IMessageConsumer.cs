using System.Threading.Tasks;

namespace Core.Utilities.MessageBrokers;

public interface IMessageConsumer
{
    Task<string> GetQueue();
}