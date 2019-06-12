using System;

namespace RAL.Collector
{
    public class TopicReceivedArgs : EventArgs
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
    }

    public class TopicHandler : ITopicMessageHandlerForProcessor
    {
        public string Topic { get; set; }

        public event EventHandler<TopicReceivedArgs> TopicReceived;

        public TopicHandler(string topic, EventHandler<TopicReceivedArgs> onReceived)
        {
            Topic = topic;
            TopicReceived += onReceived;
        }

        public void Process(string payload)
        {
            TopicReceived?.Invoke(this, new TopicReceivedArgs() { Payload = payload, Topic = Topic });
        }
    }
}
