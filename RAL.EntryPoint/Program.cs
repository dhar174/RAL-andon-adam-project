using CSScriptLib;
using RAL.Manager.Configuration;
using Serilog;
using System;
using System.IO;

namespace RAL.EntryPoint
{
    class Program
    {
        static void Main(string[] args)
        {
            string assemblyPath = Path.GetDirectoryName(AppContext.BaseDirectory);


            var StartupLogPath = $"{assemblyPath}{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}Start Ups{Path.DirectorySeparatorChar}RAL-Startup-.log";

            string startuploggeroutputtemplate = "{Timestamp: yyyy-MM-dd HH:mm:ss} [{Level: u3}] {Message: lj}{NewLine}{Exception}";

            ILogger StartUpLogger = new LoggerConfiguration()
                .Enrich.WithDemystifiedStackTraces()
                .MinimumLevel.Debug()
                .WriteTo.Async(a => a.Console(outputTemplate: startuploggeroutputtemplate))
                .WriteTo.Async(a => a.File(StartupLogPath, outputTemplate: startuploggeroutputtemplate, rollingInterval: RollingInterval.Month))
                .CreateLogger();

            StartUpLogger.Information("RAL System Launched");

            //ServicePointManager.DefaultConnectionLimit = 100;

            IConfiguration Configure = new Configuration(StartUpLogger);
            IUserConfig UserConfigurationTest;
            
            try
            {
                StartUpLogger.Information("Compiling User Configuration File");
                UserConfigurationTest = CSScript.Evaluator
                        .ReferenceAssemblyByName("RAL.Devices.Derived")
                        .LoadFile<IUserConfig>($"{assemblyPath}{Path.DirectorySeparatorChar}UserConfiguration.cs");
                StartUpLogger.Information("Compiling User Configuration File Complete");
            }
            catch (Exception ex)
            {
                StartUpLogger.Fatal(ex,"Issue Compiling User Configuration");
                throw;
            }

            //var UserConfigurationTest = new UserConfigurationTest();

            try
            {
                StartUpLogger.Information("Loading User Configuration...");
                UserConfigurationTest.UserConfigLoad();
                StartUpLogger.Information("Loading User Configuration Complete");
                StartUpLogger.Information("Validating User Configuration");
                UserConfigurationTest.Validate();
                StartUpLogger.Information("Validating User Configuration Complete");
            }
            catch (Exception ex)
            {
                StartUpLogger.Fatal(ex, "Error occurred while loading user configuration");
                StartUpLogger.Fatal("Closing RAL service");

                //** This is a hack for now as Serilog does not support calling CloseAndFlush on not static loggers
                Log.Logger = StartUpLogger;
                Log.CloseAndFlush();
                return;
            }

            Manager.Manager Manager;

            try
            {
                StartUpLogger.Information("RAL System Loading System Configuration...");
                Configure.LoadLoggerConfiguration();
                Configure.LoadConfiguration(UserConfigurationTest);
                StartUpLogger.Information("RAL System Done Loading System Configuration");

                StartUpLogger.Information("RAL Building Domain Objects...");
                Manager = Configure.ManagerFactory.Build();
                StartUpLogger.Information("RAL Done Building Domain Objects");

                StartUpLogger.Information("RAL System Starting... ");
                Manager.Start();
                StartUpLogger.Information("RAL System has Started");
                Manager.WaitForClose();
            }
            catch (Exception ex)
            {
                StartUpLogger.Fatal(ex, "Error occurred while loading system configuration");
                StartUpLogger.Fatal("Closing RAL service");

                //** This is a hack for now as Serilog does not support calling CloseAndFlush on not static loggers
                Log.Logger = StartUpLogger;
                Log.CloseAndFlush();
                return;
            }
        }
    }
}
