using RAL.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Manager
{
    public interface ISingleMachineRule<MachineType> : IRule
    {
        MachineType Machine { get; }
    }
}
