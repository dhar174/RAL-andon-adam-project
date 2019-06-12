using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RAL.Collector
{
    public class MessageProcessor : IMessageProcessorInComing
    {
        private BlockingCollection<MQTTMessage> IncomingMessages = new BlockingCollection<MQTTMessage>();

        private CancellationTokenSource cts;
        private CancellationToken ct;

        private Task _processingTask;

        private ILogger _logger;

        private IList<IClientForMQTTCollector> _listofClients = new List<IClientForMQTTCollector>();

        private ICollection<ITopicMessageHandlerForProcessor> _listOfTopics = new List<ITopicMessageHandlerForProcessor>();

        public MessageProcessor(ILogger logger = null)
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;

            _logger = logger;
        }

        public async Task RequestCloseAsync()
        {
            await Task.Run(() => Close());
        }

        /*
        public MessageProcessor(ILogger logger = null)
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;

            _logger = logger;

            _listOfTopics = listOfTopics;

        }
        */

        public void AddClientsWithTopics(IEnumerable<IClientForMQTTCollector> clients)
        {
            foreach(var client in clients)
            {
                AddClientWithTopics(client);
            }
            
        }

        public void AddClientWithTopics(IClientForMQTTCollector client)
        {
            _listofClients.Add(client);

            AddTopicHandlers(client.SubscribableTopics);
        }

        public void AddTopicHandlers(IList<ITopicMessageHandlerForProcessor> handlers)
        {
            foreach(var handler in handlers)
            {
                AddTopicHandler(handler);
            }
        }

        public void AddTopicHandler(ITopicMessageHandlerForProcessor handler)
        {
            //** Creating a list of distinct machines means that multiple machines can share the same MQTT client 

            _listOfTopics.Add(handler);
            /*
            if(!_listOfPresses.Any(x => handler.Machine == x))
            {
                _listOfPresses.Add(handler.Machine);
            }*/
        }

        public void ClientConnected(string ClientID)
        {
            var clients = _listofClients.Where(x => x.ClientId == ClientID).ToList();
            if(!(clients is null))
            {
                foreach(var m in clients)
                {
                    m.IsConnected = true;
                }
                
            }
            else
            {
                _logger?.Here(nameof(MessageProcessor),"").Warning("Unknown Client Connected");
            }
            
        }

        public void ClientDisconnected(string ClientID)
        {
            var machines = _listofClients.Where(x => x.ClientId == ClientID);
            if (!(machines is null))
            {
                foreach (var m in machines)
                {
                    m.IsConnected = false;
                }
                    
            }
            else
            {
                _logger?.Here(nameof(MessageProcessor)).Warning("Unknown Client Disconnected");
            }
        }

        /// <summary>
        /// Add Incoming Messages to Queue
        /// Interface: IMessageProcessingInComing
        /// </summary>
        public void AddMessage(MQTTMessage message)
        {
            IncomingMessages.Add(message);
        }

        public void Start()
        {
            _processingTask = Task.Run(() => DoProcessing());
        }

        private void DoProcessing()
        {
            _logger.Here(nameof(MessageProcessor)).Information("Message Processing Started");
            //** TODO This can be converted to run each iteration asynchronously, at a later date.
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    _logger.Here(nameof(MessageProcessor)).Information("Message Processing Request Stop");
                    ct.ThrowIfCancellationRequested();
                }

                if (IncomingMessages.Count == 0)
                {
                    _logger.Here(nameof(MessageProcessor)).Debug("Message Processor Waiting for More Messages");
                }
                MQTTMessage message = IncomingMessages.Take();

                var topics = _listOfTopics.Where(x => x.Topic == message.Topic).ToList();
                if (topics != null && topics.Count > 0)
                {
                    //IPressStatusData data = press.ConvertPayload(message.Payload);
                    foreach(var topic in topics)
                    {
                        try
                        {
                            //_logger?.Verbose("Processing Message Payload {Payload}", message.Payload);
                            topic.Process(message.Payload);
                        }
                        catch (NotImplementedException ex) when(ex.Message.Contains("Topic Handler"))
                        {
                            _logger.Here(nameof(MessageProcessor)).Warning(ex,"Exception Throw");
                        }
                        catch (ArgumentOutOfRangeException ex) when (ex.Message.Contains("Topic Handler"))
                        {
                            _logger.Here(nameof(MessageProcessor)).Warning(ex, "Exception Throw");
                        }
                        catch (Exception ex)
                        {
                            _logger.Here(nameof(MessageProcessor)).Error(ex, "Unknown Exception Throw");
                        }

                    }                        
                }
                else
                {
                    _logger.Here(nameof(MessageProcessor)).Warning("Message Received With a topic ({Topic}) that is not subscribed to", message.Topic);
                }
            }
        }


        public void Close()
        {
            cts.Cancel();
        }

        public void WaitUntilClosed()
        {
             ct.WaitHandle.WaitOne();
        }

        public async Task WaitUntilClosedAsync()
        {
            _logger.Here(nameof(MessageProcessor)).Debug("Asynchronously waiting for Message Processor to Close");
            await ct.WaitHandle.WaitOneAsync();
        }

    }
}
