using System.Collections.Generic;
using ADCore.Kafka.Settings;

namespace ADCore.Kafka.Scheduling
{
    public interface IWorkerDataProvider
    {
        IDictionary<string, object> Provide(WorkerOptions option);
    }
}
