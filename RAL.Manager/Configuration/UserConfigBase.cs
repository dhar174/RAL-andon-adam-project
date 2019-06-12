using RAL.Devices.StackLights;
using System;
using System.Collections.Generic;

namespace RAL.Manager.Configuration
{
    public abstract class UserConfigBase : IUserConfig
    {
        public List<MachineConfiguration> MachineConfigs { get; private set; } = new List<MachineConfiguration>();

        public List<LightToMachineMapConfiguration> LightToMachineMapConfigs { get; private set; } = new List<LightToMachineMapConfiguration>();

        public DatabaseConfiguration DatabaseConfiguration { get; protected set; }

        public EmailServerConfiguration EmailServerConfiguration { get; protected set;}

        public List<EmailReportConfig> EmailReportConfigs { get; private set; } = new List<EmailReportConfig>();

        protected string _defaultDepartment;

        public int MachineIsRunningGracePeriodInSeconds { get; protected set; }

        /// <summary>
        /// The method for the user to define the configuration for the System
        /// </summary>
        public abstract void UserConfigLoad();

        /// <summary>
        /// Validate the variables written 
        /// </summary>
        public virtual void Validate()
        {
            if(MachineConfigs is null)
            {
                throw new ArgumentNullException($"{nameof(MachineConfigs)}", $"{nameof(MachineConfigs)} Can't be null. {nameof(AddMachine)} must be used at least one in {nameof(UserConfigLoad)}");
            }

            if(LightToMachineMapConfigs is null)
            {
                throw new ArgumentNullException($"{nameof(LightToMachineMapConfigs)}", $"{nameof(LightToMachineMapConfigs)} Can't be null. {nameof(AddLightToMachineMap)} must be used at least one in {nameof(UserConfigLoad)}");
            }

            if(EmailReportConfigs is null)
            {
                throw new ArgumentNullException($"{nameof(EmailReportConfigs)}", $"{nameof(EmailReportConfigs)} Can't be null. {nameof(AddEmailReport)} must be used at least one in {nameof(UserConfigLoad)}");
            }

        }

        /// <summary>
        /// Add/Define a Machine/Press
        /// </summary>
        /// <param name="line">Name Of Production Line</param>
        /// <param name="name">Name Of Machine in the Production Line</param>
        /// <param name="mac">MAC Address for the Adam Module</param>
        /// <example> 
        /// This sample shows how to call the <see cref="AddMachine"/> method.
        /// <code>
        /// var Z11 = AddMachine("Z-1-1","Press", "FF-FF-FF-FF-FF-FF");
        /// </code>
        /// </example>
        /// <returns name="MachineConfig">
        /// For use in other Configuration Types
        /// </returns>
        protected MachineConfiguration AddMachine(string line, string name, string mac, string department = null)
        {
            if(department is null)
            {
                department = _defaultDepartment;
            }
            var machineConfig = new MachineConfiguration(line, name, mac, department);
            MachineConfigs.Add(machineConfig);
            return machineConfig;

        }
        

        protected StackLightConfiguration AddStackLight(string IPAddress, string Name)
        {
            return new StackLightConfiguration(IPAddress, Name);
        }

        /// <summary>
        /// Add/Define Light assignment for the Machine/Press
        /// </summary>
        /// <param name="Machine">Should be Variable returned from AddMachine</param>
        /// <param name="LightNumberFromTop"> Light Number On the stack light for the machine to use. Light0-Light4</param>
        /// <param name="StackLightIPAddress"></param>
        /// <example> 
        /// This sample shows how to call the <see cref="AddLightToMachineMap"/> method.
        /// <code>
        /// var Z11 = AddMachine("Z-1-1","Press", "FF-FF-FF-FF-FF-FF");
        /// var Row1StackLight = "192.168.1.100"
        /// AddLightToMachineMap(Z11, StackLight5Lights.LightNumber.Light0, Row1StackLight);
        /// </code>
        /// </example>
        protected void AddLightToMachineMap(MachineConfiguration Machine, StackLight5Lights.LightNumber LightNumberFromTop, StackLightConfiguration StackLight)
        {
            LightToMachineMapConfigs.Add(new LightToMachineMapConfiguration(Machine, LightNumberFromTop, StackLight));
        }


        protected void AddEmailReport(string[] toEmailAddressesForHourly, string[] toEmailAddressesForShiftly, Shift shift, string department = null)
        {
            if (department is null)
            {
                department = _defaultDepartment;
            }
            EmailReportConfigs.Add(new EmailReportConfig(department, shift, toEmailAddressesForHourly, toEmailAddressesForShiftly));
        }

    }
}
