using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Publisher;
using System;
using System.Collections.Generic;

namespace ADCore.Router.CallManager.Models
{
    [SubscribableMessage("RespondTopic")]
    public class ResponseModel: IPublishableMessage<ResponseModel>
    {
         public bool IsSuccessCall { get; set; }
         public string CallStatus { get; set; }

        public string TopicName { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
         public string Data { get; set; }

        public ResponseModel MapTo()
        {
            return this;
        }
    }
}
