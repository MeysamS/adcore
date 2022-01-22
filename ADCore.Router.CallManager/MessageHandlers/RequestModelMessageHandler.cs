using ADCore.ApiReader.Models;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Handler;
using ADCore.Kafka.Models;
using System.Threading.Tasks;

namespace ADCore.Router.CallManager.MessageHandlers
{
    [SubscribableMessageHandler("Msghandler")]
    public class RequestModelMessageHandler : IMessageHandler<ADCore.Router.CallManager.Models.RequestModel>
    {
        public RequestModelMessageHandler()
        {
        }

        public async Task<HandlerResult> HandleAsync(ADCore.Router.CallManager.Models.RequestModel message)
        {
            if (Jobs.CallJob.RequestQueue == null) Jobs.CallJob.RequestQueue = new System.Collections.Generic.List<Models.RequestModel>();

            CallManager.Jobs.CallJob.RequestQueue.Add(message);
            return HandlerResult.Success();
        }
    }
}