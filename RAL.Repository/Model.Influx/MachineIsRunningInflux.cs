using System;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository.Model
{
    public class MachineIsRunningInflux : MachineInfoInflux
    {
        [InfluxTimestamp]
        public DateTime Time { get; set; }

        //public IMachine Machine { get; set; }
        [InfluxField("IsRunning")]
        public bool IsRunning { get; set; }

        public MachineIsRunningInflux() : base() { }

        public MachineIsRunningInflux(IMachineInfo machine) : base (machine)
        {

        }

        public override string ToString()
        {
            return $"{Time} = {IsRunning}";
        }
    }
}
