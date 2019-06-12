using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Collector
{
    public delegate void TopicPayloadProcessDo(string topic, string payload);

    public class DelegateTopicHandler : ITopicMessageHandlerForProcessor
    {
        public string Topic { get; set; }

        public IClientForMQTTCollector Machine { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private TopicPayloadProcessDo _handler;

        public DelegateTopicHandler(string topic, TopicPayloadProcessDo handler)
        {
            Topic = topic;
            _handler = handler;
        }

        public void Process(string payload)
        {
            _handler.Invoke(Topic, payload);
        }
    }
}
