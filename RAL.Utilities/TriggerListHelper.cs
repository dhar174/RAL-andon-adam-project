using System.Collections.Generic;
using System.Text;

namespace TheColonel2688.Utilities
{
    public static class TriggerListHelper
    {
        public static string ConvertToString<T>(this IEnumerable<T> triggers)
        {
            var stringBuilder = new StringBuilder();

            var firstDone = false;
            foreach(var trigger in triggers)
            {
                if (firstDone)
                {
                    stringBuilder.Append(", ");
                }
                else
                {
                    firstDone = true;
                }                
                stringBuilder.Append(trigger);
                
            }

            return stringBuilder.ToString();
        }
    }


}
