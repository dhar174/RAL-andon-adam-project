using System;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository.Model
{
    public class MachineStatusInflux : MachineInfoInflux
    {
        [InfluxTimestamp]
        public DateTime Time { get; set; }

        [InfluxField("IsCycling")]
        public bool IsCycling { get; set; }

        [InfluxField("IsFaulted")]
        public bool IsFaulted { get; set; }

        [InfluxField("IsInAutomatic")]
        public bool IsInAutomatic { get; set; }

        public MachineStatusInflux() : base() { }

        public MachineStatusInflux(IMachineInfo machineInfo) : base (machineInfo)
        {

        }

    }
}
