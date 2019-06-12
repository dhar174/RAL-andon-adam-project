using RAL.Devices.Adam;
using RAL.Devices.StackLights;
using RAL.Manager;
using RAL.Manager.Configuration;
using RAL.Manager.Rules;
using RAL.Reports;
using RAL.Repository;
using RAL.Rules;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TheColonel2688.Utilities;

namespace RAL.EntryPoint
{
    public class Configuration : ConfigurationBase
    {
        public string assemblyPath = Path.GetDirectoryName(AppContext.BaseDirectory);

        ILogger mainLogger;

        ILogger reportLogger;

        ILogger collectorLogger;

        ILogger rulesLogger;

        ILogger repositoryLogger;

        ILogger machineLogger;


        bool _logFilesAreBuffer = true;
        bool _logFilesAreShared = false;

        TimeSpan _flushToDiskEvery = TimeSpan.FromSeconds(2);
        int _maxLogFileSize = 524288000;
        int _maxRetainedLogFileCount = 30;

        private string _stackLightLogPath;
        private string _adamClientLogPath;
        private string _rulesLogPath;
        private string _repositoryLogPath;
        private string _logRootPath;
        private string _machineLogPath;

        public Configuration(ILogger startUpLogger) : base(startUpLogger)
        {
        }


        protected ILogger GetIndividualLogger(ILogger parentLogger, LogEventLevel minimumLevel, string RootDir, string Type, string Name, bool shouldBuffer, bool shouldBeShared, TimeSpan flushToDiskInterval, int retainedFileCountLimit, int fileSizeLimitBytes)
        {
            var LogAllFile = $"{RootDir}{Path.DirectorySeparatorChar}{Name}{Path.DirectorySeparatorChar}RAL-{Type}-{Name}-.log";
            var LogWarningFile = $"{RootDir}{Path.DirectorySeparatorChar}{Name}{Path.DirectorySeparatorChar}Warnings{Path.DirectorySeparatorChar}RAL-{Type}-{Name}-.log";

            ILogger logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(a => a.Logger(parentLogger))
                .WriteTo.Async(a => a.File(LogAllFile, restrictedToMinimumLevel: minimumLevel, outputTemplate: SerilogHereHelper.TemplateForHere, 
                rollingInterval: RollingInterval.Day, buffered: shouldBuffer, shared: shouldBeShared, rollOnFileSizeLimit: true, flushToDiskInterval: flushToDiskInterval, retainedFileCountLimit: 30))
                .WriteTo.Async(a => a.File(LogWarningFile, restrictedToMinimumLevel: LogEventLevel.Warning, outputTemplate: SerilogHereHelper.TemplateForHere, 
                rollingInterval: RollingInterval.Day, buffered: shouldBuffer, shared: shouldBeShared, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes, flushToDiskInterval: flushToDiskInterval, retainedFileCountLimit: retainedFileCountLimit))
                .CreateLogger();
            return logger;
        }


        public override void LoadLoggerConfiguration()
        {
            var shouldBuffer = true;
            _logRootPath = $"{assemblyPath}{Path.DirectorySeparatorChar}Logs";

            var mainLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Everything{Path.DirectorySeparatorChar}RAL-Main-.log";
            var mainWarningLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Everything-Warnings{Path.DirectorySeparatorChar}RAL-Warnings-.log";

            _machineLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Machines";

            _stackLightLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}StackLights";

            _adamClientLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}AdamClients";

            _rulesLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Rules";

            _repositoryLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Repository";


            mainLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Console(outputTemplate: SerilogHereHelper.TemplateForHere)
                .WriteTo.Async(a => a.File(mainLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day))
                .WriteTo.Async(a => a.File(mainWarningLogPath, restrictedToMinimumLevel: LogEventLevel.Warning, 
                    outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day, 
                    buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                .CreateLogger();

            var ReportLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Reports{Path.DirectorySeparatorChar}RAL-Reports-.log";

            reportLogger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.WithDemystifiedStackTraces()
                    .WriteTo.Async(a => a.Logger(mainLogger))
                    .WriteTo.Async(a => a.File(ReportLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day,
                        buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                    .CreateLogger();

            var CollectorLogPath = $"{_logRootPath}{Path.DirectorySeparatorChar}Collector{Path.DirectorySeparatorChar}RAL-Collector-.log";

            collectorLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(a => a.Logger(mainLogger))
                .WriteTo.Async(a => a.File(CollectorLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day,
                    buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                .CreateLogger();


            var RepositoryLogPath = $"{_repositoryLogPath}{Path.DirectorySeparatorChar}RAL-Repository-.log";

            repositoryLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(a => a.Logger(mainLogger))
                .WriteTo.Async(a => a.File(RepositoryLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day, 
                    buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                .CreateLogger();

            var RulesLogPath = $"{_rulesLogPath}{Path.DirectorySeparatorChar}RAL-Rules-.log";

            rulesLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(a => a.Logger(mainLogger))
                .WriteTo.Async(a => a.File(RulesLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day, 
                    buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                .CreateLogger();

            
            var MachineLogPath = $"{_machineLogPath}{Path.DirectorySeparatorChar}RAL-All-Machines-.log";

            machineLogger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(a => a.Logger(mainLogger))
                .WriteTo.Async(a => a.File(MachineLogPath, outputTemplate: SerilogHereHelper.TemplateForHere, rollingInterval: RollingInterval.Day, 
                buffered: shouldBuffer, rollOnFileSizeLimit: true, fileSizeLimitBytes: _maxLogFileSize, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount))
                .CreateLogger();



        }

        public override void LoadConfiguration(IUserConfig userConfig)
        {
            Stopwatch sw = Stopwatch.StartNew();

            DatabaseConfiguration databaseConfiguration = userConfig.DatabaseConfiguration;

            if(databaseConfiguration is null)
            {
                databaseConfiguration = new DatabaseConfiguration("172.16.28.250", "RALSystem", "1234", "TRMI_RAL_System");
            }

            MachineRepositoryCache MachineRepository = new MachineRepositoryCache(
                ipaddress: databaseConfiguration.IPAddress,
                username: databaseConfiguration.Username,
                password: databaseConfiguration.Password,
                timeOut: TimeSpan.FromMilliseconds(10000),
                databaseName: databaseConfiguration.DatabaseName,
                logger: repositoryLogger);

            ManagerFactory.WithLoggerForCollectorManager(collectorLogger);
            ManagerFactory.WithLoggerForRulesManager(rulesLogger);
            ManagerFactory.WithLoggerForReportManager(reportLogger);
            ManagerFactory.WithLoggerForManager(StartUpLogger);

            var listOfStackLightConfigs = userConfig.LightToMachineMapConfigs.Select(x => x.StackLight).Distinct();

            var listOfStackLights = new List<SignaworksEthernetAsStackLightRYGBW>();

            foreach(var stackLightConfig in listOfStackLightConfigs)
            {
                listOfStackLights.Add(new SignaworksEthernetAsStackLightRYGBW(stackLightConfig.IPAddress, logger: GetIndividualLogger(parentLogger: mainLogger, minimumLevel: LogEventLevel.Debug,
                    RootDir: _stackLightLogPath, Type: "Stack Light", Name: stackLightConfig.Name, shouldBuffer: _logFilesAreBuffer, 
                    shouldBeShared: _logFilesAreShared, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount, fileSizeLimitBytes: _maxLogFileSize)));
            }

            //var RepositoryForRules = new RepositoryForRules(MachineRepository);

            int MachineIsRunningGracePeriodInSeconds = userConfig.MachineIsRunningGracePeriodInSeconds;

            var machines = new List<Machine>();

            Stopwatch machinesw = new Stopwatch();

            foreach (var machine in userConfig.MachineConfigs)
            {

                machinesw.Restart();
                StartUpLogger.Here(nameof(Configuration)).Debug("Loading Config for machine {Line}.{Name}", machine.Line, machine.Name);
                var machineInfo = new MachineInfo()
                {
                    MAC = machine.MAC,
                    Line = machine.Line,
                    Name = machine.Name,
                    Department = machine.Department
                };

                var AdamClientLoggers = new Dictionary<string, ILogger>();

                var AdamClientLogger = AdamClientLoggers.FirstOrDefault(x => x.Key == "MAC").Value;

                if(AdamClientLogger is null)
                {
                    AdamClientLogger = GetIndividualLogger(parentLogger: machineLogger, minimumLevel: LogEventLevel.Information, RootDir: _adamClientLogPath, Type: "Adam Client", Name: machineInfo.MAC, shouldBuffer: _logFilesAreBuffer, 
                        shouldBeShared: _logFilesAreShared, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount, fileSizeLimitBytes: _maxLogFileSize);
                    AdamClientLoggers.Add(machineInfo.MAC, AdamClientLogger);
                }

                var Press = new Machine(
                    machineInfo,
                    MachineRepository,
                    new Adam6051Client(machineInfo.MAC, AdamClientLogger),
                    machine.MQTTPayloadConverter,
                    logger: GetIndividualLogger(parentLogger: machineLogger, minimumLevel: LogEventLevel.Information, RootDir: _machineLogPath, Type: "Machine", Name: machineInfo.ToString(), shouldBuffer: _logFilesAreBuffer, 
                    shouldBeShared: _logFilesAreShared, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount, fileSizeLimitBytes: _maxLogFileSize)
                );

                var tempRuleMachineIsRunning = new RuleMachineIsRunning(machine: Press, TimeSpan.FromSeconds(MachineIsRunningGracePeriodInSeconds), 
                    pollInterval: TimeSpan.FromMilliseconds(1000), repository: MachineRepository, 
                    logger: GetIndividualLogger(parentLogger: rulesLogger, minimumLevel: LogEventLevel.Information, RootDir: _rulesLogPath, Type: "Rule", Name: $"{machine.LineName}-IsRunning", shouldBuffer: _logFilesAreBuffer, 
                    shouldBeShared: _logFilesAreShared, flushToDiskInterval: _flushToDiskEvery, retainedFileCountLimit: _maxRetainedLogFileCount, fileSizeLimitBytes: _maxLogFileSize));

                try
                {
                    LightToMachineMapConfiguration LightToMachineMap = userConfig.LightToMachineMapConfigs.First(x => x.Machine == machine);
                    var StackLight = listOfStackLights.First(x => x.IPAddress == LightToMachineMap.StackLight.IPAddress);
                    tempRuleMachineIsRunning.AddAction(new RuleActionStackLightOnWhenIsRunningFalse(StackLight, LightToMachineMap.LightNumberFromTop));
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no matching element"))
                {
                    StartUpLogger.Fatal($"Can not find LightToMachine Map for Machine {machine.LineName}. This is currently required for all machines.");
                    throw;
                }

                tempRuleMachineIsRunning.AddAction(new RuleActionWriteIsRunning(MachineRepository, logger: rulesLogger));
                Press.AddRule(tempRuleMachineIsRunning);
                machines.Add(Press);
                machinesw.Stop();
                StartUpLogger.Here(nameof(Configuration)).Debug("Done Loading Configuration for Machine {Line}.{Name}, Took {elapsed}", machine.Line, machine.Name, machinesw.Elapsed);
            }

            ManagerFactory.AddMachines(machines);


            var EmailServer = userConfig.EmailServerConfiguration.EmailServer; //"smtp.ipower.com";

            var EmailServerPort = userConfig.EmailServerConfiguration.EmailServerPort; // 587;

            var FromEmailAddress = "RAL-System-noreply@tramgroup.com";

            var Credentials = userConfig.EmailServerConfiguration.Credentials; //("betz@betzmachine.com", "Betz-320");

            var PathToReportTemplate = $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml";

            var RepositoryForReports = new RepositoryForReports(MachineRepository);

            var StartTime = (hour: 00, minute: 30);

            if(userConfig.EmailReportConfigs is null)
            {
                throw new ArgumentNullException($"{nameof(userConfig.EmailReportConfigs)}",$"{nameof(userConfig.EmailReportConfigs)} Can't be null. {"AddEmailReport()"} must be used at least one in {nameof(UserConfigBase.UserConfigLoad)}");
            }

            Stopwatch swreport = new Stopwatch();

            foreach (var reportConfig in userConfig.EmailReportConfigs)
            {
                var ShiftStartHour = 0;
                var ShiftEndHour = 0;
                var ShiftAsString = "Error";

                swreport.Restart();
                switch (reportConfig.Shift)
                {
                    case Shift.First:
                        ShiftStartHour = 6;
                        ShiftEndHour = 14;
                        ShiftAsString = "1st";
                        break;
                    case Shift.Second:
                        ShiftStartHour = 14;
                        ShiftEndHour = 22;
                        ShiftAsString = "2nd";
                        break;
                    case Shift.Third:
                        ShiftStartHour = 22;
                        ShiftEndHour = 6;
                        ShiftAsString = "3rd";
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Shift Specified");
                }
                
                for (int i = ShiftStartHour; i < ShiftEndHour; i++)
                {
                    (int hour, int minute) tempStart = (StartTime.hour + i, StartTime.minute);
                    (int hour, int minute) tempEnd = (StartTime.hour + i + 1, StartTime.minute);

                    var tempReport = new DepartmentTimePeriodReportForCurrentDay(
                    from: FromEmailAddress,
                    emailAddresses: reportConfig.ToEmailAddressesForHourly,
                    department: reportConfig.Department,
                    smtpServerHostName: EmailServer,
                    smtpServerPort: EmailServerPort,
                    smtpServerCredentials: Credentials,
                    pathToTemplate: PathToReportTemplate,
                    repository: MachineRepository,
                    logger: reportLogger)
                    {
                        Name = $"Report for hour {tempStart.hour:00}:{tempStart.minute:00} - {tempEnd.hour:00}:{tempEnd.minute:00} for {reportConfig.Department}",
                        StartTime = tempStart,
                        EndTime = tempEnd
                    };

                    ManagerFactory.AddReport(tempReport, (s) => s.ToRunEvery(0).Days().At(tempEnd.hour, tempEnd.minute));
                }

                var shiftReport = new DepartmentTimePeriodReportForCurrentDay(
                    from: FromEmailAddress,
                    emailAddresses: reportConfig.ToEmailAddressesForShiftly,
                    department: reportConfig.Department,
                    smtpServerHostName: EmailServer,
                    smtpServerPort: EmailServerPort,
                    smtpServerCredentials: Credentials,
                    pathToTemplate: PathToReportTemplate,
                    repository: MachineRepository,
                    logger: reportLogger)
                {
                    Name = $"{ShiftAsString} Shift Report for {reportConfig.Department}",
                    StartTime = (Hour: ShiftStartHour, Minute: 30),
                    EndTime = (Hour: ShiftEndHour, Minute: 30)
                };
                /*
                var secondShiftReport = new DepartmentTimePeriodReportForCurrentDay(
                    from: FromEmailAddress,
                    emailAddresses: reportConfig.ToEmailAddressesForShiftly,
                    department: reportConfig.Department,
                    smtpServerHostName: EmailServer,
                    smtpServerPort: EmailServerPort,
                    smtpServerCredentials: Credentials,
                    pathToTemplate: PathToReportTemplate,
                    repository: MachineRepository,
                    logger: reportLogger)
                {
                    Name = $"2nd Shift Report for {reportConfig.Department}",
                    StartTime = (Hour: 14, Minute: 30),
                    EndTime = (Hour: 22, Minute: 30)
                };

                var thirdShiftReport = new DepartmentTimePeriodReportForCurrentDay(
                    from: FromEmailAddress,
                    emailAddresses: reportConfig.ToEmailAddressesForShiftly,
                    department: reportConfig.Department,
                    smtpServerHostName: EmailServer,
                    smtpServerPort: EmailServerPort,
                    smtpServerCredentials: Credentials,
                    pathToTemplate: PathToReportTemplate,
                    repository: MachineRepository,
                    logger: reportLogger)
                {
                    Name = $"3rd Shift Report for {reportConfig.Department}",
                    StartTime = (Hour: 22, Minute: 30),
                    EndTime = (Hour: 06, Minute: 30)
                };
                */
                ManagerFactory.AddReport(shiftReport, (s) => s.ToRunEvery(0).Days().At(ShiftEndHour, 30));

                //ManagerFactory.AddReport(secondShiftReport, (s) => s.ToRunEvery(0).Days().At(22, 30));

                //ManagerFactory.AddReport(thirdShiftReport, (s) => s.ToRunEvery(0).Days().At(06, 30));

                swreport.Stop();
                StartUpLogger.Here(nameof(Configuration)).Debug("Done Loading Configuration for report {Department}, Took {elapsed}", reportConfig.Department, swreport.Elapsed);
            }


            sw.Stop();
            StartUpLogger.Here(nameof(Configuration)).Debug("Done Loading Configuration, Took {elapsed}", sw.Elapsed);
        }

    }
}
