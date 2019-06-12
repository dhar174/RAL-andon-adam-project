using System;
using System.Collections.Generic;

namespace RAL.ConfigStorageTypes
{
    public class MachineConfigDSC
    {
        public string Name { get; set; }
        public string Line { get; set; }
        public string MAC { get; set; }
        public string Department { get; set; }

        public Type MachineType { get; set; }
        public Type PayloadConverterType { get; set; }

        public IList<RuleIntervalDSC> Rules { get; set; }
    }
}
