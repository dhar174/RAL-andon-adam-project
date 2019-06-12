using Serilog;

namespace RAL.Manager.Configuration
{
    public interface IConfiguration
    {
        ManagerFactory ManagerFactory { get; set; }

        ILogger StartUpLogger { get; }

        void LoadLoggerConfiguration();

        void LoadConfiguration(IUserConfig userConfig);

    }
}
