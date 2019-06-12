using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models.Responses;
using RAL.Repository;
using RAL.TrendTriggers;

namespace RAL.TrendTrigger.TestInfluxDatanet
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            var client  = new InfluxDbClient("http://192.168.1.208:8086/", "TRMI_Trends", "1234", InfluxData.Net.Common.Enums.InfluxDbVersion.Latest);

            //var QueryHelper = new InfluxDataHelper();

            //client.
            List<string> queries = new List<string>();
            var testQuery1 = InfluxDataHelper.BuildQueryLimitDesc(2, "Z-1-2");
            var testQuery2 = InfluxDataHelper.BuildQueryLimitDesc(2, "Z-1-3");
            //queries.Add(testQuery1);
            var results1 = Task.Run(() => client.Client.QueryAsync(testQuery1, "TRMI_Production_Dev"));
            var results2 = Task.Run(() => client.Client.QueryAsync(testQuery2, "TRMI_Production_Dev"));
            Task.WaitAll(results1, results2);
            foreach (var serie in results1.Result)
            {
                foreach(var tag in serie.Columns)
                {
                    Console.Write($"{tag}, ");
                }
                Console.WriteLine();

                foreach (var seriesValue in serie.Values)
                {
                    foreach(var value in seriesValue)
                    {

                        Console.Write($"{value}, ");
                    }
                    Console.WriteLine();
                }
            }

            foreach (var blah in results2.Result)
            {
                foreach (var babyblah in blah.Tags)
                {
                    Console.WriteLine($"{babyblah.Key} : {babyblah.Value}");
                }
            }
        }
    }
}
