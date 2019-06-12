using RAL.Collector.Mocks.Converter;
using RAL.Collector.Mocks.Repository;
using RAL.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;


namespace RAL.Collector.ConsoleTest
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            //StartRepoMockTest();

            ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            await RunTestNonConfig(logger, null);

        }


        public static async Task RunTestNonConfig(ILogger logger, string logFilePath)
        {
            string rootPath;
            string loggingPath;
            if (logFilePath is null)
            {
                rootPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}{Path.DirectorySeparatorChar}RAL Project Debug Logs";
                loggingPath = $"{rootPath}{Path.DirectorySeparatorChar}CollectorStandAlone{Path.DirectorySeparatorChar}";
            }
            else
            {
                rootPath = logFilePath;
                loggingPath = $"{rootPath}{Path.DirectorySeparatorChar}Collector{Path.DirectorySeparatorChar}";
            }

            //var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine} at {Type}:{MemberName}{NewLine}{Exception}{NewLine}";

            ILogger sublogger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Logger(logger))
                .WriteTo.Async(a => a.File($"{loggingPath}Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log").MinimumLevel.Verbose())
                .MinimumLevel.Verbose()
                .CreateLogger();


            await RunTestNotLoadedFromFileTest(sublogger);

        }



        public static async Task RunTestWithConfig(ILogger logger, string logFilePath, string ConfigDir)
        {
            string rootPath;
            string loggingPath;
            if (logFilePath is null)
            {
                rootPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}{Path.DirectorySeparatorChar}RAL Project Debug Logs";
                loggingPath = $"{rootPath}{Path.DirectorySeparatorChar}CollectorStandAlone{Path.DirectorySeparatorChar}";
            }
            else
            {
                rootPath = logFilePath;
                loggingPath = $"{rootPath}{Path.DirectorySeparatorChar}Collector{Path.DirectorySeparatorChar}";
            }

            //var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine} at {Type}:{MemberName}{NewLine}{Exception}{NewLine}";

            ILogger sublogger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Logger(logger))
                .WriteTo.Async(a => a.File($"{loggingPath}Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log").MinimumLevel.Verbose())
                .MinimumLevel.Verbose()
                .CreateLogger();


            await RunTestWithLoadedConfig(sublogger, ConfigDir);

        }

        private static async Task RunTestWithLoadedConfig(ILogger sublogger, string ConfigDir)
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            DirectoryInfo appDir = Directory.GetParent(Assembly.GetEntryAssembly().Location);
            DirectoryInfo appParentDir = new DirectoryInfo(appDir.Parent.FullName);
            DirectoryInfo appLogDir = new DirectoryInfo($"{appParentDir.FullName}{Path.DirectorySeparatorChar}logs");


            string rootPath = ConfigDir; //$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}{Path.DirectorySeparatorChar}";
            string repositoryConfigPath = $"{rootPath}{Path.DirectorySeparatorChar}repository.xml.config";
            string machineSubscriptionsPath = $"{rootPath}{Path.DirectorySeparatorChar}machines.xml.config";


            var configMananger = new RepositoryFactory().LoadFrom(repositoryConfigPath);

            var influxServerAddress = new Uri($"http://{configMananger.Config.DatabaseIPaddress}:{configMananger.Config.DatabasePort}");

            var ping = new Ping();

            var reply = ping.Send(influxServerAddress.Host);

            if(reply.Status != IPStatus.Success)
            {
                sublogger.Fatal("Ping to Database Server Failed. Exiting...");
                return;
            }

            var repo = new MachineRepository(configMananger.Config.DatabaseIPaddress, databaseName: configMananger.Config.DatabaseName, port: Convert.ToInt32(configMananger.Config.DatabasePort));

            if (!repo.CanConnect())
            {
                sublogger.Fatal("Can't Connect to DB Server. Exiting...");
                return;
            }

            var _pressWriterInflux = new RepositoryWriterPressInflux(repo, sublogger);
            
            var _messageProcessor = new MessageProcessor(sublogger);

            var machines = new CollectorManagerFactory().LoadFromFile(machineSubscriptionsPath).WithRepoWriter(_pressWriterInflux).WithLogger(sublogger).Build();

            var topicHandlers = new TopicHandlerFactory().AddMachines(machines).Build();

            _messageProcessor.AddMachineTopicHandlers(topicHandlers);

            var _collector = new MQTTCollector(_messageProcessor, sublogger);

            Console.WriteLine("Starting Collection");

            _collector.Start();

            _messageProcessor.StartProcessing();

            Console.WriteLine("Waiting indefinitely");
            await _collector.WaitUntilStoppedAsync();
        }


        private static async Task RunTestNotLoadedFromFileTest(ILogger sublogger)
        {

            string IPAddress = "192.168.10.11";
            string MAC = "00D0C9FCA9BA";

           // var influxServerAddress = new Uri("http://192.168.1.208:8086");

            var repo = new MachineRepository("192.168.1.208");

            if (!repo.CanConnect())
            {
                sublogger.Fatal("Can't Connect to DB Server. Exiting...");
                return;
            }

            var _pressWriterInflux = new RepositoryWriterPressInflux(repo, sublogger);

            var realPress = new PressAdam6051(
                new Repository.Model.MachineInfoInflux()
                {
                    IPAddress = IPAddress,
                    MAC = MAC,
                    Line = "Y-1-1",
                    Department = Tag.Value.DepartmentPartsProduction,
                    Name = "Y-1-1"
                },
                 _pressWriterInflux,
                 new PressAdam6051PayloadConverterStatus(),
                 sublogger
                );


            var _pressList = new List<PressAdam6051>();
            _pressList.Add(realPress);

            
            for (int i = 2; i < 13; i++)
            {
                _pressList.Add(new PressAdam6051(new Repository.Model.MachineInfoInflux()
                {
                    IPAddress = IPAddress,
                    MAC = "00D0C9FCA9BA",
                    Line = $"Z-1-{i}",
                    Department = Tag.Value.DepartmentPartsProduction,
                    Name = $"Z-1-{i}",
                }, _pressWriterInflux, new PressAdam6051MessageConverterMock(i),
                sublogger)
                );
            }
            
            //var _listOfTopicHandlers = new List<ITopicMessageHandler>();

            var PressTopicHandlerForData = new TopicHandler(realPress.TopicForStatus, realPress);

            //_listOfTopicHandlers.Add(PressTopicHandlerForData);

            var _messageProcessor = new MessageProcessor(sublogger);

            _messageProcessor.AddMachineTopicHandler(PressTopicHandlerForData);

            foreach (var press in _pressList)
            {
                _messageProcessor.AddMachineTopicHandler(new TopicHandler(press.TopicForStatus, press));
            }

            var _collector = new MQTTCollector(_messageProcessor, sublogger);

            Console.WriteLine("Starting Collection");
            //await _collector.StartAsync();

            _collector.Start();

            _messageProcessor.StartProcessing();

            Console.WriteLine("Waiting indefinitely");
            await _collector.WaitUntilStoppedAsync();

        }

        static void StartRepoMockTest()
        {
            string _loggingPath = $"{System.Reflection.Assembly.GetExecutingAssembly().Location}Collector{Path.DirectorySeparatorChar}";

            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console().MinimumLevel.Information()
                .CreateLogger();


            string IPAddress = "192.168.10.11";
            string MAC = "00D0C9FCA9BA";

            var _pressWriterInflux = new RepositoryWriterMachineMock(
                (machine, IsConnected) =>
                {
                    logger.Information($"Mock Connection Write: {machine.Line}-{machine.MAC}-{IsConnected}");
                },
                (machine, IsCycling) =>
                {
                    logger.Information($"Mock Status Write: {machine.Line}-{machine.MAC}-{IsCycling}");
                    return DateTime.UtcNow;
                }
                );

            var realPress = new PressAdam6051(
                new Repository.Model.MachineInfoInflux() {
                    IPAddress = IPAddress,
                    MAC = MAC,
                    Line = "Y-1-1",
                    Department = Tag.Value.DepartmentPartsProduction,
                    Name = "BC-80020001-0000" },
                 _pressWriterInflux,
                 new PressAdam6051PayloadConverterStatus(),
                 logger
                );


            var _pressList = new List<PressAdam6051>();
            _pressList.Add(realPress);
            
            for (int i = 2; i < 13; i++)
            {
                _pressList.Add(new PressAdam6051(new Repository.Model.MachineInfoInflux()
                {
                    IPAddress = IPAddress,
                    MAC = "00D0C9FCA9BA",
                    Line = $"Z-1-{i}",
                    Department = Tag.Value.DepartmentPartsProduction,
                    Name = $"BC-801{"0000":i}-0000",
                }, _pressWriterInflux, new PressAdam6051MessageConverterMock(i), logger)
                );
            }
            
            //var _listOfTopicHandlers = new List<ITopicMessageHandler>();

            var PressTopicHandlerForData = new TopicHandler(realPress.TopicForStatus, realPress);

            //_listOfTopicHandlers.Add(PressTopicHandlerForData);

            var _messageProcessor = new MessageProcessor(logger);

            _messageProcessor.AddMachineTopicHandler(PressTopicHandlerForData);

            foreach(var press in _pressList)
            {
                _messageProcessor.AddMachineTopicHandler(new TopicHandler(press.TopicForStatus, press));
            }

            var _collector = new MQTTCollector(_messageProcessor, logger);

            _collector.Start();

            _messageProcessor.StartProcessing();

            _collector.WaitUntilStopped();

        }
    }
}
