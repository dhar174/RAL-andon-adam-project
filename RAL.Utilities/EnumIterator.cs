using System;
using System.Collections.Generic;
using System.Linq;

namespace TheColonel2688.Utilities
{
    public static class EnumIterator
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}