using DeepEqual.Syntax;
using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RAL.Repository.Tests
{
    public class StatusRepoTests
    {
        [Fact]
        public async void GetLastOrDefault()
        {

            var testDB = $"{nameof(StatusRepoTests)}.{nameof(GetLastOrDefault)}";

            var repo = new MachineRepositoryCache("192.168.1.208", "TestRunner","1234", TimeSpan.FromMilliseconds(5000), testDB);

            if (repo.DoesDBExist(testDB))
            {
                repo.DropDB(testDB);
            }

            repo.CreateDB(testDB);



            var machines = new List<MachineInfoInflux>();

            for (int i = 0; i < 40; i++)
            {
                var machine = new MachineInfoInflux()
                {
                    MAC = $"MAC{i+1}",
                    IPAddress = $"IP{i+1}",
                    Department = "Dept1",
                    Name = "Press",
                    Line = $"Y-1-{i+1}"
                };

                machines.Add(machine);
            }

            var rows = new List<MachineStatusInflux>();

            var IsCycling = false;
            for(int a = 0; a < 100; a++)
            {
                
                foreach(var machine in machines)
                {
                    var machineIsRunning = new MachineStatusInflux(machine)
                    {
                        IsCycling = IsCycling,
                        IsInAutomatic = true,
                        IsFaulted = false,
                        Time = DateTime.UtcNow
                    };
                    await repo.MachineStatusRepo.WriteAsync(machineIsRunning);
                    rows.Add(machineIsRunning);
                    IsCycling = !IsCycling;
                }
                
            }

            foreach (var machine in machines)
            {
                var dbresult = await repo.MachineIsRunningRepo.LastOrDefaultAsync(machine.Line, machine.Name);
                var listResult = rows.LastOrDefault(x => x.Line == machine.Line && x.Name == machine.Name);
                if (listResult is null || dbresult is null)
                {
                    continue;
                }

                dbresult.WithDeepEqual(listResult).Assert();
            }

            repo.DropDB(testDB);

        }



        [Fact]
        public async void LastOrDefaultWhereIs()
        {

            var testDB = $"{nameof(StatusRepoTests)}.{nameof(LastOrDefaultWhereIs)}";

            var repo = new MachineRepositoryCache("192.168.1.208", "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), testDB);

            if (repo.DoesDBExist(testDB))
            {
                repo.DropDB(testDB);
            }

            repo.CreateDB(testDB);



            var machines = new List<MachineInfoInflux>();

            for (int i = 0; i < 40; i++)
            {
                var machine = new MachineInfoInflux()
                {
                    MAC = $"MAC{i + 1}",
                    IPAddress = $"IP{i + 1}",
                    Department = "Dept1",
                    Name = "Press",
                    Line = $"Y-1-{i + 1}"
                };

                machines.Add(machine);
            }

            var rows = new List<MachineStatusInflux>();

            var IsCycling = false;
            for (int a = 0; a < 100; a++)
            {

                foreach (var machine in machines)
                {
                    var machineIsRunning = new MachineStatusInflux(machine)
                    {
                        IsCycling = IsCycling,
                        IsInAutomatic = true,
                        IsFaulted = false,
                        Time = DateTime.UtcNow
                    };
                    await repo.MachineStatusRepo.WriteAsync(machineIsRunning);
                    rows.Add(machineIsRunning);
                    IsCycling = !IsCycling;
                }

            }

            foreach (var machine in machines)
            {
                var dbresult = await repo.MachineStatusRepo.LastOrDefaultWhereIsAsync(machine.Line, machine.Name, true, true, false);
                var listResult = rows.LastOrDefault(x => x.Line == machine.Line && x.Name == machine.Name && x.IsCycling == true && x.IsInAutomatic == true && x.IsFaulted == false);


                dbresult.WithDeepEqual(listResult).Assert();
            }

            repo.DropDB(testDB);


        }
    }
}
