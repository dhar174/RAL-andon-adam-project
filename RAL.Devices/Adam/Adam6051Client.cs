using RAL.Collector;
using RAL.Collector.MessageProcessing;
using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Collections.Generic;

namespace RAL.Devices.Adam
{
    public class Adam6051PayloadReceivedEventArgs : EventArgs
    {
        public string Payload { get; private set; }

        public Adam6051PayloadReceivedEventArgs(string payload)
        {
            Payload = payload;
        }
    }

    public class Adam6051Client : ClientForCollector, IClientForMQTTCollector
    {
        private const string topicRoot = "Advantech";
        private const string subTopicForStatus = "data";
        private const string subTopicLastWill = "Device_Status";


        public string LastIPAddress { get; set; }
        private string _mac;
        public string MAC
        {
            get { return _mac; }
            set
            {
                _mac = value;
                _macWithNoSeparators = value.Replace("-", "").Replace(":", "");
            }
        }

        public event EventHandler<Adam6051PayloadReceivedEventArgs> StatusReceived;

        public event EventHandler<Adam6051PayloadReceivedEventArgs> LastWillReceived;

        private bool _isConnected;

        public override bool IsConnected
        {
            get => _isConnected;
            set
            {
                ThrowIfNotInitialized();
                _isConnected = value;
                if (value)
                {
                    RaiseConnected();
                }
                else
                {
                    RaiseDisconnected();
                }
            }
        }

        protected string _macWithNoSeparators;

        private ILogger _logger;

        private string TopicForStatus { get => $"{ topicRoot}/{_macWithNoSeparators}/{subTopicForStatus}"; }

        private string TopicForLastWill { get => $"{topicRoot}/{_macWithNoSeparators}/{subTopicLastWill}"; }

        public override string ClientId { get => $"ADAM6051_{_macWithNoSeparators}"; }

        public override IList<ITopicMessageHandlerForProcessor> SubscribableTopics => 
            new List<ITopicMessageHandlerForProcessor> {
                new TopicHandler(TopicForStatus, OnReceived),
                new TopicHandler(TopicForLastWill, OnReceived)
            };

        /// <summary>
        /// This is only for the Message Processor to use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void OnReceived(object sender, TopicReceivedArgs eventArgs)
        {
            ProcessPayload(eventArgs.Topic, eventArgs.Payload);
        }
             
        public Adam6051Client(string mac, ILogger logger = null)
        {
            Initialize(mac, logger);
        }
        
        public bool IsInitialized()
        {
            try
            {
                ThrowIfNotInitialized();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void ThrowIfNotInitialized()
        {
            if (MAC is null)
            {
                throw new InvalidOperationException("MAC cannot be null");
            }
        }

        public override void ProcessPayload(string topic, string payload)
        {
            ThrowIfNotInitialized();

            switch (topic)
            {
                case var blah when (blah == TopicForStatus):
                    StatusReceived?.Invoke(this, new Adam6051PayloadReceivedEventArgs(payload));
                    break;
                case var blah when (blah == TopicForLastWill):
                    LastWillReceived?.Invoke(this, new Adam6051PayloadReceivedEventArgs(payload));
                    break;
                default:
                    //** TODO Add logging and better error message
                    throw new ArgumentOutOfRangeException($"{nameof(Adam6051Client)}: Topic Handler for {topic} is does not exist.");
            }
        }

        public void Initialize(string mac, ILogger logger)
        {
            _logger = logger;

            MAC = mac;

            _logger?.Here(nameof(Adam6051Client), MAC).Information("Adam6051 Client {MAC} has been initialized. (Not connected yet)", mac);
        }

        public override void RaiseConnected()
        {
            _logger.Here(nameof(Adam6051Client), MAC).Information("Connected");
            base.RaiseConnected();
        }

        public override void RaiseDisconnected()
        {
            _logger.Here(nameof(Adam6051Client), MAC).Information("Disconnected");
            base.RaiseDisconnected();
        }
    }
}
