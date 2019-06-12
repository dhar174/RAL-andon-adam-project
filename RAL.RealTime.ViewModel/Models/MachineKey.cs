using System;

namespace RAL.RealTime.Models
{
    public class MachineKey
    {
        

        public string Line { get; set; }
        public string Name { get; set; }


        public MachineKey(string line, string name)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}