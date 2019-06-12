using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace RAL.Manager.Configuration
{
    public abstract class ConfigurationBase : IConfiguration
    {
        public ManagerFactory ManagerFactory { get; set; }

        public ILogger StartUpLogger { get; set; }

        public ConfigurationBase(ILogger startUpLogger)
        {
            StartUpLogger = startUpLogger;
            ManagerFactory = new ManagerFactory();
        }

        //protected abstract ILogger GetIndividualMachineLogger(ILogger parentLogger, string RootMachineLogDir, string machineName);

        public abstract void LoadConfiguration(IUserConfig userConfig);

        public abstract void LoadLoggerConfiguration();
    }
}
