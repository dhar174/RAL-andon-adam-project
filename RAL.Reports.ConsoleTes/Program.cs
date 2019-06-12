using FluentScheduler;
using RAL.Repository;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RAL.Reports.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
            string reportsConfigPath = $"{rootPath}{Path.DirectorySeparatorChar}reports.xml.config";


            var configMananger = new RepositoryFactory().LoadFrom(repositoryConfigPath);

            var influxServerAddress = new Uri($"http://{configMananger.Config.DatabaseIPaddress}:{configMananger.Config.DatabasePort}");

            var repo = new MachineRepository(configMananger.Config.DatabaseIPaddress, databaseName: configMananger.Config.DatabaseName, port: Convert.ToInt32(configMananger.Config.DatabasePort));

            if (!repo.CanConnect())
            {
                sublogger.Fatal("Can't Connect to DB Server. Exiting...");
                return;
            }


            var reports = new ReportFactory().LoadFromFile(reportsConfigPath).WithRepository(new RepositoryForReports(repo)).WithLogger(sublogger).Build();

            var registry = new Registry();

            foreach (var report in reports)
            {
                registry.Schedule(() => report.Execute()).ToRunEvery(0).Days().At(report.EndTime.Hours,report.EndTime.Minutes + 1);
            }

        }

    }
}
