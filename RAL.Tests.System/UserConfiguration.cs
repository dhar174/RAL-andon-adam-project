using RAL.Manager.Configuration;
using RAL.Devices.StackLights;
using RAL.Devices.Adam.Mocks;

//namespace RAL.Tests.Integration
//{
    public class UserConfigurationTest : UserConfigBase
    {
        public override void UserConfigLoad()
        {
            //** Define Machines/Presses and the MAC address for the ADAM Module
            //** Example
            //** var Z11 = AddMachine("Z-1-1","Press","FF-FF-FF-FF-FF")

            _defaultDepartment = "Test Department";

            MachineIsRunningGracePeriodInSeconds = 90;

            EmailServerConfiguration = new EmailServerConfiguration()
            {
                EmailServer = "smtp.ipower.com",
                EmailServerPort = 587,
                Credentials = ("betz@betzmachine.com", "Betz-320")
            };

            DatabaseConfiguration = new DatabaseConfiguration("172.16.28.250", "RALSystem", "1234", "TRMI_RAL_System_Dev");

            var listOfStackLightConfigs = new StackLightConfiguration[]
            {
                AddStackLight("172.16.28.151", "Row 1 Stack Light"),
                AddStackLight("172.16.28.152", "Row 2 Stack Light"),
                AddStackLight("172.16.28.153", "Row 3 Stack Light"),
                AddStackLight("172.16.28.154", "Row 4 Stack Light"),
                AddStackLight("172.16.28.155", "Row 5 Stack Light"),
                AddStackLight("172.16.28.156", "Row 6 Stack Light"),
                AddStackLight("172.16.28.157", "Row 7 Stack Light"),
                AddStackLight("172.16.28.158", "Row 8 Stack Light")
            };

            int Converter = 0;
            int StackLightNumber = 0;
            int LightOnStackLight = 0;

            for (int i = 0; i < 40; i++)
            {
                Converter = Converter < 8 ? Converter : 0;
                StackLightNumber = LightOnStackLight < 5 ? StackLightNumber : ++StackLightNumber;
                LightOnStackLight = LightOnStackLight < 5 ? LightOnStackLight : 0;

                string MAC = "00-D0-C9-FC-A9-BA";
                if(i >= 36)
                {
                    if (i == 36)
                    {
                        Converter = 0;
                    }
                    MAC = "00-D0-C9-FC-A9-C0";
                }
                var Press = new MachineConfiguration($"Test Line {i + 1}", "Press", MAC, _defaultDepartment, new RAL.Devices.Adam.Mocks.Adam6051StatusConverterForPressMock(Converter++ + 1));
                MachineConfigs.Add(Press);

                AddLightToMachineMap(Press, (StackLight5Lights.LightNumber)LightOnStackLight++, listOfStackLightConfigs[StackLightNumber]);
            }


            AddEmailReport(new [] {"bnewman@betzmachine.com"}, new[] { "bnewman@betzmachine.com" });
        }
    }
//}
