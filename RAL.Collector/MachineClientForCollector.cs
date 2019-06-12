using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RAL.Collector.MessageProcessing
{
    public abstract class ClientForCollector
    {
        public virtual string ClientId { get; set; }

        public abstract IList<ITopicMessageHandlerForProcessor> SubscribableTopics { get; }

        public abstract bool IsConnected { get; set; }

        public event EventHandler<AsyncCompletedEventArgs> Connected;

        public event EventHandler<AsyncCompletedEventArgs> Disconnected;

        //public event EventHandler<TopicReceivedArgs> TopicReceived;

        public ClientForCollector()
        {

        }

        public virtual void RaiseConnected()
        {
            Connected?.Invoke(this, null);
        }

        public virtual void RaiseDisconnected()
        {
            Disconnected?.Invoke(this, null);
        }

        public abstract void ProcessPayload(string topic, string payload);
    }
}
