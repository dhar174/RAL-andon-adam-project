using System;
using System.Collections.Generic;

namespace RAL.Collector
{
    public interface IClientForMQTTCollector
    {
        string ClientId { get; set; }

        bool IsConnected { get; set; }

        void OnReceived(object sender, TopicReceivedArgs eventArgs);

        IList<ITopicMessageHandlerForProcessor> SubscribableTopics { get; }
    }
}

