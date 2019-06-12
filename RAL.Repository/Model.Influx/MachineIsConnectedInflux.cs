using System;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository.Model
{
    public class MachineIsConnectedInflux : MachineInfoInflux
    {
        [InfluxTimestamp]
        public DateTime Time { get; set; }

        [InfluxField("IsConnected")]
        public bool IsConnected { get; set; }

    }
}
