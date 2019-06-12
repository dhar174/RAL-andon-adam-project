using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Collector
{
    public class MQTTMessage
    {
        public string ClientId { get; set; }
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}
