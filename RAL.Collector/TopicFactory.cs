using System.Collections.Generic;
using Serilog;

namespace RAL.Collector
{
    
    public class TopicHandlerFactory
    {

        IList<ITopicMessageHandlerForProcessor> listOfTopics = new List<ITopicMessageHandlerForProcessor>();

        private ILogger _logger;

        public TopicHandlerFactory(ILogger logger = null)
        {
            _logger = logger;
        }
        /*
        public TopicHandlerFactory AddMachines(IEnumerable<IClientForMQTTCollector> machines)
        {
            foreach(var machine in machines)
            {
                AddClient(machine);
            }

            return this;
        }
        */
        /*
        public TopicHandlerFactory AddClient(IClientForMQTTCollector machine)
        {

            foreach(var topic in machine.SubscribableTopics)
            {
                listOfTopics.Add(new TopicHandler(topic, machine.OnReceived));
            }
            
            return this;
        }
        */
        public IList<ITopicMessageHandlerForProcessor> Build()
        {
            return listOfTopics;
        }


    }
    
}
