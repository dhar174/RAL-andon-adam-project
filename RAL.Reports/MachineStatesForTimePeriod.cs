using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAL.Reports
{
    public class MachineStatesForTimePeriod<T>
    {
        public DateTime Start => States.First().Start;

        public DateTime End => States.Last().End;

        public IList<StateOverTime<T>> States = new List<StateOverTime<T>>();

        public (string Line, string Name) MachineInfo;

    }
}
