using System.Collections.Generic;
using System.Text;

namespace RAL.Manager.Configuration
{
    public interface IUserConfig
    {
        List<MachineConfiguration> MachineConfigs { get; }

        List<LightToMachineMapConfiguration> LightToMachineMapConfigs { get; }

        DatabaseConfiguration DatabaseConfiguration { get; }

        List<EmailReportConfig> EmailReportConfigs { get; }

        EmailServerConfiguration EmailServerConfiguration { get; }

        int MachineIsRunningGracePeriodInSeconds { get; }

        void Validate();

        void UserConfigLoad();
    }
}
