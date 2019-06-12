using System;
using System.Collections.Generic;
using System.Text;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository.Model
{
    public class MachineInfoInflux : IMachineInfo
    {
        [InfluxTag("IPAddress")]
        public string IPAddress { get; set; }
        [InfluxTag("MAC")]
        public string MAC { get; set; }
        [InfluxTag("Line")]
        public string Line { get; set; }
        [InfluxTag("Department")]
        public string Department { get; set; }
        [InfluxTag("Name")]
        public string Name { get; set; }


        public MachineInfoInflux()
        {

        }

        public MachineInfoInflux(IMachineInfo machine)
        {
            IPAddress = machine.IPAddress;
            MAC = machine.MAC;
            Line = machine.Line;
            Name = machine.Name;
            Department = machine.Department;
        }
    }
}
