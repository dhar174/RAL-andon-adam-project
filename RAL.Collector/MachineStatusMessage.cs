using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Vibrant.InfluxDB.Client;

namespace RAL.Collector
{
    public class MachineStatusMessage
    {
        public bool IsCycling { get; set; }

        public bool IsInAutomatic { get; set; }

        public bool IsFaulted { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is MachineStatusMessage))
            {
                return false;
            }
            var comparing = (MachineStatusMessage)obj;

            return IsCycling == comparing.IsCycling && IsInAutomatic == comparing.IsInAutomatic && IsFaulted == comparing.IsFaulted;
        }

        public override int GetHashCode()
        {
            var hashCode = -1698028535;
            hashCode = hashCode * -1521134295 + IsCycling.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInAutomatic.GetHashCode();
            hashCode = hashCode * -1521134295 + IsFaulted.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{nameof(IsCycling)}: {IsCycling}";
        }

        public MachineStatusMessage Copy()
        {
            return new MachineStatusMessage() { IsCycling = IsCycling, IsFaulted = IsFaulted, IsInAutomatic = IsInAutomatic };
        }
    }
}
