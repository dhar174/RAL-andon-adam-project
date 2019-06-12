using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Repository
{
    public class InfluxDataHelper
    {

        public static string BuildQueryLimitDesc(int number, string measurement, string Line, string Name = null)
        {
            string query = "";

            query = $"SELECT * FROM \"{measurement}\" WHERE \"Line\" = '{Line}' ";

            if(Name != null)
            {
                query = $"{query} AND \"Name\" = '{Name}'"; 
            }
            /*
            query = $"{query} AND \"IsCycling\" =";
            
            if (IsCycling)
            {
                query = $"{query} 'true'";
            }
            else
            {
                query = $"{query} 'false'";
            }
            */

            return query = $"{query} ORDER BY time DESC LIMIT {number}";
            
        }


        public static string BuildQueryLimitDesc(int number, string measurement, string Line, string Name, DateTime before)
        {
            string query = "";

            query = $"SELECT * FROM \"{measurement}\" WHERE \"Line\" = '{Line}' ";


            query = $"{query} AND \"Name\" = '{Name}'";

            query = $"{query} AND time < '{before.ToString("yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'")}'";

            /*
            query = $"{query} AND \"IsCycling\" =";
            
            if (IsCycling)
            {
                query = $"{query} 'true'";
            }
            else
            {
                query = $"{query} 'false'";
            }
            */

            return query = $"{query} ORDER BY time DESC LIMIT {number}";

        }



        public string BuildQueryForIsLineRunningOld(TimeSpan timeout, string Line, string Machine = null)
        {

            if (Machine is null)
            {
                return $"SELECT 'IsCycling' FROM 'Status' WHERE 'Line' = {Line} AND time < now() - {timeout.Seconds}s";
            }
            else
            {
                return $"SELECT 'IsCycling' FROM 'Status' WHERE \"Line\" = '{Line}' AND \"Machine\" = '{Machine}' AND time < now() - {timeout.Seconds}s";
            }
        }

        internal static DateTime GetValue(IList<string> columns, IList<object> row, object time)
        {
            throw new NotImplementedException();
        }

        public string BuildQueryForLastStatusWhereTagEquals(string Tag, string TagValue)
        {
            return $"SELECT * WHERE '{Tag}' = \"{TagValue}\" ORDER BY time DESC LIMIT 1";

        }

        public static object GetValue(IList<string> Columns, IList<object> values, string ColumnName)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i] == ColumnName)
                {
                    return values[i];
                }              
            }
            //** TODO Better Logging and exception
            throw new ArgumentException();
        }
    }
}
