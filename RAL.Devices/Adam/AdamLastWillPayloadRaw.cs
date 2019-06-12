using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Devices.Adam
{
    public class AdamLastWillPayloadRaw
    {
        public string status { get; set; }
        public string name { get; set; }
        public string macid { get; set; }
        public string ipaddress { get; set; }
    }
}
