using System.Threading.Tasks;

namespace ADCore.Kafka.Messaging.Handler
{

    /// <summary>
    /// Keep this per-scope.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandlerManager<THandler, TMessage>
        where THandler : IMessageHandler<TMessage>
    {
        Task ManageAsync();
    }
}
