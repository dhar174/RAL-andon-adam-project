using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB.Client.Rows;

namespace InfluxDBConnectionBenc
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.Write("Server IP or hostname: ");
                var host = Console.ReadLine();
                Console.Write("Number of Pings: ");
                var numberOfPings = Console.ReadLine();


                var client = new InfluxClient(new Uri($"http://{host}:8086"), "RALSystem", "1234");

                var IsRunningROw = new DynamicInfluxRow();
                IsRunningROw.Fields.Add("MAC", "Hello");
                IsRunningROw.Fields.Add("Line", "Test");
                IsRunningROw.Tags.Add("Name", "Test");
                IsRunningROw.Tags.Add("IsRunning", "true");
                IsRunningROw.Timestamp = DateTime.Now;



                var StatusRow = new DynamicInfluxRow();
                StatusRow.Fields.Add("MAC", "Hello");
                StatusRow.Fields.Add("Line", "Test");
                StatusRow.Tags.Add("Name", "Test");
                StatusRow.Tags.Add("IsCycling", "true");
                StatusRow.Timestamp = DateTime.Now;


                try
                {
                    client.WriteAsync("TRMI_RAL_System_Dev", "IsRunning", new[] { IsRunningROw }).GetAwaiter().GetResult();

                    client.WriteAsync("TRMI_RAL_System_Dev", "Status", new[] { StatusRow }).GetAwaiter().GetResult();

                    var result  = client.ReadAsync<DynamicInfluxRow>("TRMI_RAL_System_Dev", "SELECT * FROM \"IsRunning\"").GetAwaiter().GetResult();

                    
                }
                catch (Exception ex)
                {                    
                    Console.WriteLine(ex.ToString());
                }


                List<Task<InfluxPingResult>> tasks = new List<Task<InfluxPingResult>>();

                Console.WriteLine("Waiting....");

                Task.Delay(1000).Wait();


                Console.WriteLine("Starting Pings");

                Stopwatch sw = new Stopwatch();

                async Task<InfluxPingResult> DoPing(int i)
                {
                    Stopwatch swa = new Stopwatch();
                    swa.Start();

                    var result = await client.PingAsync();

                    swa.Stop();
                    Console.WriteLine($"[{i}] {sw.Elapsed}");
                    return result;
                }

                sw.Start();

                for (int i = 0; i < Convert.ToInt32(numberOfPings); i++)
                {
                    tasks.Add(DoPing(i));
                }

                Task.WaitAll(tasks.ToArray());

                sw.Stop();

                Console.WriteLine($"Total Time was {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"Press Any Key to Exit");
            Console.ReadKey();

        }
    }
}
