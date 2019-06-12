using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace RAL.Collector
{

    /// <summary>
    /// Load
    /// </summary>
    public class CollectorManager
    {
        private MQTTCollector _collector;

        private MessageProcessor _messageProcessor;

        //private IRepositoryWriterMachine _repositoryWriterMachine;

        private IList<IClientForMQTTCollector> _clients;

        private ILogger _logger;

        public CollectorManager(IList<IClientForMQTTCollector> clients, ILogger logger = null)
        {
            _clients = clients;
            _logger = logger;
        }

        public async Task RequestCloseAsync()
        {
            Task collectorStopTask = _collector.StopAsync();
            Task MessageProcessorStopTask = _messageProcessor.RequestCloseAsync();

            await Task.WhenAll(collectorStopTask, MessageProcessorStopTask);
        }

        public async Task WaitForCloseToCompleteAsync()
        {
            if(_collector is null)
            {
                return;
            }
            Task collectorStopTask = _collector.WaitUntilClosedAsync();
            Task MessageProcessorStopTask = _messageProcessor.WaitUntilClosedAsync();

            await Task.WhenAll(collectorStopTask,MessageProcessorStopTask);
        }

        public void WaitForCloseToComplete()
        {
            _collector.WaitUntilClosed();
            _messageProcessor.WaitUntilClosed();
        }

        public void Start()
        {
            if(_clients.Count < 1)
            {
                throw new InvalidOperationException($"{nameof(CollectorManager)} is Not Initialized. No clients have been added.");
            }

            _messageProcessor = new MessageProcessor(_logger);

            _messageProcessor.AddClientsWithTopics(_clients);

            _collector = new MQTTCollector(_messageProcessor, _logger);

            _collector.Start();

            _messageProcessor.Start();
        }

    }
}
