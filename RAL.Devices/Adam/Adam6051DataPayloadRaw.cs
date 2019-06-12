using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Devices.Adam
{
    public class Adam6051DataPayloadRaw
    {
        public int s { get; set; }
        public int t { get; set; }
        public int c { get; set; }
        public bool di1 { get; set; }
        public bool di2 { get; set; }
        public bool di3 { get; set; }
        public bool di4 { get; set; }
        public bool di5 { get; set; }
        public bool di6 { get; set; }
        public bool di7 { get; set; }
        public bool di8 { get; set; }
        public bool di9 { get; set; }
        public bool di10 { get; set; }
        public bool di11 { get; set; }
        public bool di12 { get; set; }
        public int di13 { get; set; }
        public int di14 { get; set; }
        public bool do1 { get; set; }
        public bool do2 { get; set; }
    }
}
