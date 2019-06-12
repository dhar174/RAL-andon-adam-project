using RAL.RealTime.Models;
using RAL.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAL.RealTime.UI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new MachineRepository("172.16.28.250", "RALSystem", "1234", TimeSpan.FromSeconds(20), "TRMI_RAL_System");
            var MachinesToSubscribeTo = new List<MachineKey>()
            {
                new MachineKey("E-6-5", "Press"),
                new MachineKey("E-6-6", "Press"),
                new MachineKey("E-7-3", "Press"),
                new MachineKey("F-2-1", "Press"),
                new MachineKey("F-3-3", "Press"),
                new MachineKey("F-4-5", "Press"),
                new MachineKey("F-4-6", "Press"),
                new MachineKey("F-4-7", "Press"),
                new MachineKey("F-6-2", "Press"),
                new MachineKey("F-6-4", "Press"),
                new MachineKey("F-7-2", "Press"),
                new MachineKey("F-7-3", "Press"),
                new MachineKey("G-4-1", "Press"),
                new MachineKey("G-4-2", "Press"),
                new MachineKey("F-2-2", "Press"),
                new MachineKey("F-3-1", "Press"),
                new MachineKey("F-6-3", "Press"),
                new MachineKey("G-6-1", "Press"),
                new MachineKey("G-6-2", "Press"),

            };
            var fetcher = new StatusFetcher(repository: repository, TimeSpan.FromMilliseconds(1000), MachinesToSubscribeTo);

            fetcher.OnStatusesUpdated += (sender, eventArgs) => { WriteStatuses(fetcher.MachineStatuses.Items); };
            fetcher.Start();
 
            System.Console.ReadKey();
        }

        static void WriteStatuses(IEnumerable<MachineStatus> statuses)
        {
            System.Console.Clear();
            foreach (var status in statuses)
            {
                System.Console.WriteLine($"{status.Line}.{status.Name} = {status.Status}");
            }
            System.Console.WriteLine("Press Any Key to Exit");
        }
    }
}
