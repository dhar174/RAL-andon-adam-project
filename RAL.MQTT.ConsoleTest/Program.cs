using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RAL.MQTT.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsServer = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(new System.Net.IPAddress(new byte[] { 192, 168, 10, 254 }))
                .WithDefaultEndpointPort(1883);


            IMqttServer _mqttBroker = new MqttFactory().CreateMqttServer();

            //IMqttClient mqttClient = new MqttFactory().CreateMqttClient();

            //var optionsClient = new MqttClientOptionsBuilder().WithClientId("9999").WithTcpServer("192.168.10.254", 1883);



            var receivedEvents = new List<string>();

            _mqttBroker.ClientConnected += delegate (object sender, MQTTnet.Server.MqttClientConnectedEventArgs e)
            {
                //receivedEvents.Add(args.ClientId);
                Console.WriteLine($"Client {e.ClientId} has connected");
            };


            _mqttBroker.ApplicationMessageReceived += delegate (object sender, MqttApplicationMessageReceivedEventArgs e)
            {
                //receivedEvents.Add(args.ClientId);
                Console.WriteLine($"Client {e.ClientId} Published ({e.ApplicationMessage.ConvertPayloadToString()})");
            };

            //** Start broker
            Task _brokerStart = Task.Run(() => _mqttBroker.StartAsync(optionsServer.Build()));
            _brokerStart.Wait();
            Console.WriteLine("Broker Started");

            //Task taskClientStart = Task.Run(() => mqttClient.ConnectAsync(optionsClient.Build()));
            //taskClientStart.Wait();

            Console.WriteLine("Waiting to exit, Press Any Key");
            var pause = new ManualResetEvent(false);

            Console.ReadKey();    

            //pause.WaitOne();
            Console.WriteLine("Goodbye");



            //var taskClientStop = Task.Run(() => mqttClient.DisconnectAsync());
            //taskClientStop.Wait();

            //** Stop Broker
            Task _brokerTaskStop = Task.Run(() => _mqttBroker.StopAsync());
            _brokerTaskStop.Wait();
            Console.WriteLine("Broker Stopped");

               
            

        }
    }
}
