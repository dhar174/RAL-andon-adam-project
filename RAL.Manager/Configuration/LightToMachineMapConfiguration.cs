using RAL.Devices.StackLights;

namespace RAL.Manager.Configuration
{
    public class LightToMachineMapConfiguration
    {
        public MachineConfiguration Machine { get; private set; }
        public string MachineLine => Machine.Line;
        public string MachineName => Machine.Name;
        public StackLight5Lights.LightNumber LightNumberFromTop { get; private set; }
        public StackLightConfiguration StackLight { get; private set; }

        public LightToMachineMapConfiguration(MachineConfiguration Machine, StackLight5Lights.LightNumber LightNumberFromTop, StackLightConfiguration StackLight)
        {
            this.Machine = Machine;
            this.LightNumberFromTop = LightNumberFromTop;
            this.StackLight = StackLight;           
        }
    }
}
