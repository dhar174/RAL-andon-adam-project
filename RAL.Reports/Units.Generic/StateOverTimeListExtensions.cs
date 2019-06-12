using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAL.Reports
{
    public static class StateOverTimeListExtensions
    {
        
        public static TimeSpan Total<T>(this IList<StateOverTime<T>> states)
        {
                return new TimeSpan(states.Sum(x => x.TimeSpan.Ticks));
        }


        public static TimeSpan TotalTimeWhere<T>(this IList<StateOverTime<T>> states, Func<StateOverTime<T>, bool> predicate)
        {
            var results = states.Where(predicate).ToList();

            var blah = results.Total();

            return blah;
        }

        public static double PercentOfTotalWhere<T>(this IList<StateOverTime<T>> states, Func<StateOverTime<T>, bool> predicate)
        {
            var results = states.Where(predicate).ToList();
            var TimeSpanInTicks = results.Total().Ticks;
            double blah = (double)TimeSpanInTicks / states.Total().Ticks;

            return blah;
        }

        /*
        public static void AddRangeMerge<T>(this IList<StateOverTime<T>> states, IList<StateOverTime<T>>[] arrayOfListOfStatesToCombine)
        {
            var tempList = new List<StateOverTime<T>>();
            foreach (var list in arrayOfListOfStatesToCombine)
            {
                tempList.AddRange(list);
            }
            //** TODO NOT DONE

        }

        */
        /*
        //private IList<StateOverTime<T>> _states = new List<StateOverTime<T>>();

        public int Count => _states.Count();

        public bool IsReadOnly => _states.IsReadOnly();

        public StateOverTime<T> this[int index] { get => _states[index]; set => _states[index]; }


        public int IndexOf(StateOverTime<T> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, StateOverTime<T> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(StateOverTime<T> item)
        {
             
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(StateOverTime<T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(StateOverTime<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(StateOverTime<T> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<StateOverTime<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        */
    }
}
