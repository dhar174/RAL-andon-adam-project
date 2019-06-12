using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAL.Reports
{
    public class MachineStateTransformer
    {
        public static IList<StateOverTime<T>> Transform<T>(DateTime timePeriodStart, DateTime timePeriodEnd, List<(DateTime Time, T State)> states, T initialState)
        {
            (DateTime Time, T State) lastEntry;

            if (!states.Any(x=> x.Time == timePeriodStart))
            {
                lastEntry.State = initialState;
                lastEntry.Time = timePeriodStart;
            }
            else
            {
                lastEntry = states.First(x => x.Time == timePeriodStart);
            }


            var TransformedStates = new List<StateOverTime<T>>();

            for (int i = 0; i < states.Count; i++)
            {
                if (!lastEntry.State.Equals(states[i].State))
                {
                    TransformedStates.Add(new StateOverTime<T>()
                    {
                        Start = lastEntry.Time,
                        End = states[i].Time,
                        State = lastEntry.State
                    });
                    lastEntry = states[i];
                }
                else
                {
                    continue;
                }

            }

            TransformedStates.Add(new StateOverTime<T>() { Start = lastEntry.Time, End = timePeriodEnd, State = lastEntry.State });
            

            return TransformedStates;
        }

        /*
        public static List<StateOverTime<bool>> Transform<bool>(DateTime shiftStartAsDateTime, DateTime shiftEndAsDateTime, List<(DateTime Time, bool IsRunning)> whenIsRunning) : When T is bool
        {
            throw new NotImplementedException();
        }
        */
    }
}
