using RAL.Collector;
using System.Collections.Generic;

namespace RAL.Manager.Configuration
{
    /// <summary>
    /// This is an immutable class for loading in configurations
    /// </summary>
    public class MachineConfiguration
    {
        public string MAC { get; private set; }
        public string Line { get; private set; }
        public string Name { get; private set; }
        public string Department { get; private set; }

        public string FullName { get => $"{Line}.{Name}"; }

        public IMachineStatusPayloadConverter MQTTPayloadConverter { get; private set; }

        public (string MachineLine, string MachineName) LineName => (Line, Name);

        public MachineConfiguration(string line, string name, string mac, string department, IMachineStatusPayloadConverter mqttPayloadConverter = null)
        {
            Line = line;
            Name = name;
            MAC = mac;
            Department = department;
            MQTTPayloadConverter = mqttPayloadConverter;


        }

    }
}
