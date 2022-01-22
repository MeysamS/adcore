using System.Collections.Generic;
using System.Threading.Tasks;
using ADCore.Kafka.Settings;

namespace ADCore.Kafka.Messaging.Handler
{
    public interface IMessagingBootstrapper
    {
        Task BootstrapAsync(MessageHandlerOptions handlerOptions, IDictionary<string, WorkerOptions> workerOptions);
    }
}
