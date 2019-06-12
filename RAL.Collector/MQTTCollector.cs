using MQTTnet;
using MQTTnet.Server;
using TheColonel2688.Utilities;
using Serilog;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RAL.Collector
{
    public class MQTTCollector
    {
        private IMqttServer _mqttBroker;

        private ILogger _logger;

        private IMessageProcessorInComing _incomingMQTTMessages;

        public event EventHandler<EventArgs> Stopped;

        private volatile bool _isRunning;

        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken ct;


        public MQTTCollector(IMessageProcessorInComing incomingMQTTMessages, ILogger logger = null)
        {
            ct = cts.Token;

            _logger = logger;
            
            _mqttBroker = new MqttFactory().CreateMqttServer();
            _mqttBroker.ClientConnected += _mqttBroker_ClientConnected;
            _mqttBroker.ClientDisconnected += _mqttBroker_ClientDisconnected;
            _mqttBroker.Stopped += _mqttBroker_Stopped;
            _mqttBroker.ApplicationMessageReceived += _mqttBroker_ApplicationMessageReceived;
 
            _incomingMQTTMessages = incomingMQTTMessages;

        }

        public async Task StartAsync()
        {
            _logger?.Information("Collector Is Starting");
            try
            {
                await _mqttBroker.StartAsync(new MqttServerOptions());
                IsRunning = true;
                _logger?.Here(nameof(MQTTCollector)).Information("Collector has Started Listening at {IP}:{Port}",
                    _mqttBroker.Options.DefaultEndpointOptions.BoundInterNetworkAddress,
                    _mqttBroker.Options.DefaultEndpointOptions.Port);
            }
            catch (SocketException ex) when (ex.Message.Contains("Only one usage of each socket address (protocol/network address/port) is normally permitted"))
            {
                _logger?.Here(nameof(MQTTCollector)).Error(ex,"A Broker Is Already Listening on {IP}:{Port}", 
                    _mqttBroker.Options.DefaultEndpointOptions.BoundInterNetworkAddress, 
                    _mqttBroker.Options.DefaultEndpointOptions.Port);
                throw;
            }

            return;
        }

        public void Start()
        {
            StartAsync().Wait();
        }

        public void Stop()
        {
           StopAsync().Wait();
        }

        public async Task StopAsync()
        {
            _logger?.Here(nameof(MQTTCollector)).Warning("MQTT Collector Stop Requested");
            await _mqttBroker.StopAsync();
            IsRunning = false;
            cts.Cancel(); //** Needed for Wait Until Stopped Method.
            return;
        }


        public async Task WaitUntilClosedAsync()
        {
            _logger?.Here(nameof(MQTTCollector)).Information("Asynchronously Waiting For MQTT Collector to Close");
            //await Task.Run(() => WaitHandle.WaitAll(new[] { ct.WaitHandle }));
            await ct.WaitHandle.WaitOneAsync();
            _logger.Here(nameof(MQTTCollector)).Warning("MQTT Collector Stopped");
        }

        public void WaitUntilClosed()
        {
            _logger?.Here(nameof(MQTTCollector)).Information("Waiting For Collector to Close (This is Thread Blocking, on the calling Thread)");
            WaitHandle.WaitAll(new[] { ct.WaitHandle });
            _logger?.Here(nameof(MQTTCollector)).Warning("Collector Stopped");
        }

        private void _mqttBroker_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //_logger.Verbose("MQTTCollector - {Client}-{Topic}-{Payload}", e.ClientId, e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString());
            _incomingMQTTMessages.AddMessage(new MQTTMessage() { Topic = e.ApplicationMessage.Topic, Payload = e.ApplicationMessage.ConvertPayloadToString(), ClientId = e.ClientId});
        }

        private void _mqttBroker_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            _logger?.Here(nameof(MQTTCollector)).Debug("Client {ClientId} Connected to Broker.", e.ClientId);
            _incomingMQTTMessages.ClientConnected(e.ClientId);
        }

        private void _mqttBroker_ClientDisconnected(object sender, MqttClientDisconnectedEventArgs e)
        {
            _logger?.Here(nameof(MQTTCollector)).Debug("Client {ClientId} Disconnected from Broker.", e.ClientId);
            _incomingMQTTMessages.ClientDisconnected(e.ClientId);
        }

        
        private void _mqttBroker_Stopped(object sender, EventArgs e)
        {
            _logger?.Here(nameof(MQTTCollector)).Warning("MQTT Broker Has Stopped");
            Stopped?.Invoke(this, e);
        }
        
    }
}
