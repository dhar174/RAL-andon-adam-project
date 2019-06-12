using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Reports
{
    public class StateOverTime<T>
    {
        public TimeSpan TimeSpan { get => End - Start; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public T State { get; set; }

        public override string ToString()
        {
            return $"[{TimeSpan}: {Start} -> {End}] = {State.ToString()}";
        }

    }

}
