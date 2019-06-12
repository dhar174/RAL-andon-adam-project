using RAL.Repository;
using RAL.RulesEngine.StackLights;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Serilog;
using RAL.Factory;

namespace RAL.RulesEngine.ConsoleTest
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            await RunTestNonConfig(null,null);


            Console.WriteLine("Press Any Key to Exit");
            Console.ReadKey();

        }

        public static async Task RunTestWithConfig(ILogger logger, string logFilePath, string configDir)
        {
            //string rootPath = configDir;
            string repositoryConfigPath = $"{configDir}{Path.DirectorySeparatorChar}repository.xml.config";
            string machineSubscriptionsPath = $"{configDir}{Path.DirectorySeparatorChar}machines.xml.config";


            string _loggingPath = $"{logFilePath}{Path.DirectorySeparatorChar}RulesEngine{Path.DirectorySeparatorChar}";

            ILogger sublogger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Logger(logger))
                .WriteTo.Async(a => a.File($"{_loggingPath}Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log").MinimumLevel.Verbose())
                .MinimumLevel.Verbose()
                .CreateLogger();

            sublogger.Information("{Class}.{Method} is Starting...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));

            RulesManager rulesManager = new RulesManager();

            var RepositoryConfig = new RepositoryFactory().LoadFrom(repositoryConfigPath).Config;


            var rulesFactory = new RulesFactory(sublogger)
                .LoadFromFile(machineSubscriptionsPath)
                .WithLogger(sublogger)
                .WithRepository(
                new RepositoryForRules(
                    new MachineRepository(
                        RepositoryConfig.DatabaseIPaddress,
                        RepositoryConfig.DatabaseName,
                        Convert.ToInt32(RepositoryConfig.DatabasePort)
                        )
                    )
                );

            var rules = rulesFactory.Build();

            rules = rulesManager.Rules = rules;


            await rulesManager.StartAsync();


            sublogger.Information("{Class}.{Method} Waiting Indefinitely...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));
            await Task.Run(() => rulesManager.WaitForStop());

            //await rulesManager.StopAsync();
            sublogger.Information("{Class}.{Method} Closing...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));


        }

        public static async Task RunTestNonConfig(ILogger logger, string logFilePath = null)
        {
            string rootPath;
            if(logFilePath is null)
            {
                rootPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
            else
            {
                rootPath = logFilePath;
            }
            

            string _loggingPath = $"{rootPath}{Path.DirectorySeparatorChar}RulesEngine{Path.DirectorySeparatorChar}";

            ILogger sublogger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Logger(logger))
                .WriteTo.Async( a => a.File($"{_loggingPath}Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log").MinimumLevel.Verbose())
                .MinimumLevel.Verbose()
                .CreateLogger();

            sublogger.Information("{Class}.{Method} is Starting...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));

            //Console.WriteLine($"{nameof(RAL)}{nameof(TrendTriggers)}{nameof(ConsoleTest)} Running...");

            //var client = new InfluxDbClient("http://192.168.1.208:8086/", "TRMI_Trends", "1234", InfluxData.Net.Common.Enums.InfluxDbVersion.Latest);

            var repo = new MachineRepository("192.168.1.208");

            IList<string> Lines = new[]{
                "Y-1-1",                
                "Z-1-2",
                "Z-1-3",
                "Z-1-4",
                "Z-1-5",
                "Z-1-6",
                "Z-1-7",
                "Z-1-8",
            };

            IList<MachineInfo> listOfMachines = new List<MachineInfo>();

            RulesManager rulesManager = new RulesManager();

            Queue<(IStackLight5Light, StackLight5Lights.LightNumber)> lightQueue = new Queue<(IStackLight5Light, StackLight5Lights.LightNumber)>();

            //var MockStackLight1 = new StackLight5LightsMock() { IPAddress = "999.999.999.1" };
            //var MockStackLight2 = new StackLight5LightsMock() { IPAddress = "999.999.999.2" };

            SignaworksEthernetStackLight SigStackLight1 = new SignaworksEthernetStackLight("192.168.10.201");
            SignaworksEthernetStackLight SigStackLight2 = new SignaworksEthernetStackLight("192.168.10.202");
            SignaworksEthernetStackLight SigStackLight3 = new SignaworksEthernetStackLight("192.168.10.203");
            SignaworksEthernetStackLight SigStackLight4 = new SignaworksEthernetStackLight("192.168.10.204");

            var StackLight1 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight1);
            var StackLight2 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight2);
            var StackLight3 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight3);
            var StackLight4 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight4);


            lightQueue.Enqueue((StackLight1, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight1, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight2, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight2, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight3, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight3, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight4, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight4, StackLight5Lights.LightNumber.Light1));

            var rulesRepo = new RepositoryForRules(repo);

            for (int i = 0; i < Lines.Count; i++)
            {
                string line = Lines[i];
                var tempMachine = new MachineInfo() { Department = Tag.Value.DepartmentPartsProduction, Line = line, Name = line, IPAddress = $"Machine{i}", MAC= "Test" };
                listOfMachines.Add(tempMachine);
                var RuleIsRunning = new RuleMachineIsRunning();
                RuleIsRunning.Initialize(TimeSpan.FromSeconds(4), TimeSpan.FromMilliseconds(500), tempMachine, rulesRepo, sublogger);
                var light = lightQueue.Dequeue();
                var RuleIsRunningLight = new RuleMachineIsRunningStackLight(tempMachine, light.Item1, light.Item2, rulesRepo, TimeSpan.FromMilliseconds(500), sublogger);
                rulesManager.Rules.Add(RuleIsRunning);
                rulesManager.Rules.Add(RuleIsRunningLight);
            }


            await rulesManager.StartAsync();


            //await Task.Delay(TimeSpan.MaxValue);

            sublogger.Information("{Class}.{Method} Waiting Indefinitely...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));
            await Task.Run(() => rulesManager.WaitForStop());

            //await rulesManager.StopAsync();
            sublogger.Information("{Class}.{Method} Closing...", $"{nameof(RAL)}{nameof(RulesEngine)}{nameof(ConsoleTest)}{nameof(Program)}", nameof(RunTestNonConfig));
        }



    }
}
