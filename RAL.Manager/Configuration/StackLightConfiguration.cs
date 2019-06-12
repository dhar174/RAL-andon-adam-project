using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Manager.Configuration
{
    public class StackLightConfiguration
    {
        public string IPAddress { get; private set; }
        public string Name { get; private set; }

        public StackLightConfiguration(string ipaddress, string name)
        {
            IPAddress  = ipaddress;
            Name = name; 
        }
    }
}
