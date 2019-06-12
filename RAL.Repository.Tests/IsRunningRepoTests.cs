using DeepEqual.Syntax;
using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RAL.Repository.Tests
{
    public class IsRunningRepoTests
    {
        [Fact]
        public async void GetLastOrDefault()
        {

            var testDB = $"{nameof(IsRunningRepoTests)}.{nameof(GetLastOrDefault)}";

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

            var rows = new List<MachineIsRunningInflux>();

            var IsRunning = false;
            for(int a = 0; a < 100; a++)
            {
                
                foreach(var machine in machines.Take(36))
                {
                    var machineIsRunning = new MachineIsRunningInflux(machine)
                    {
                        IsRunning = IsRunning,
                        Time = DateTime.UtcNow
                    };
                    await repo.MachineIsRunningRepo.WriteAsync(machineIsRunning);
                    rows.Add(machineIsRunning);
                    IsRunning = !IsRunning;
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
        public void GetDataForTimeRangeReportAsync()
        {
            var ShiftStart = (Hours: 07, Minutes: 00);
            var ShiftEnd = (Hours: 15, Minutes: 00);

            var StartDateTime = DateTime.Now.Date.AddHours(ShiftStart.Hours).AddMinutes(ShiftStart.Minutes);

            var EndDateTime = DateTime.Now.Date.AddHours(ShiftEnd.Hours).AddMinutes(ShiftEnd.Minutes);


            var TestDB = $"{nameof(IsRunningRepoTests)}.{nameof(GetDataForTimeRangeReportAsync)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository("192.168.1.208", "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

            
            try
            {
                var listOfMachines = new (string Line, string Name)[]
                {
                    ("W-1-1", Name:"Press"),                   
                    ("W-1-2", Name: "Press"),
                    ("W-1-3", Name: "Press"),
                    ("W-1-4", Name: "Press"),
                    ("W-1-5", Name: "Press"),
                    ("W-1-7", Name: "Press"),
                    ("W-1-8", Name: "Press"),
                    ("W-1-9", Name: "Press"),
                    ("W-1-10", Name: "Press"),
                    ("W-2-1", Name: "Press"),
                    ("W-2-2", Name: "Press"),
                    ("W-2-3", Name: "Press")
                };

                var dataToSend = new List<MachineIsRunningInflux>();
                foreach (var machine in listOfMachines)
                {
                    var list = GenerateMachineIsRunningData(testDepartment, machine, ShiftStart, ShiftEnd, 8);
                    dataToSend.AddRange(list);
                }

                var dataToSendSorted = dataToSend.OrderBy(x => x.Line).OrderBy(x => x.Name).OrderBy(x => x.Time);

                repo.MachineIsRunningRepo.WriteAsync(dataToSendSorted).Wait();
                var dataBack = (List<MachineIsRunningInflux>)repo.MachineIsRunningRepo.GetDataForTimeRangeReportAsync(StartDateTime, EndDateTime, testDepartment).Result;


                // dataToSend.Sort((x, y) => x.Time.CompareTo(y.Time));

                var dataBackSorted = dataBack.OrderBy(x => x.Line).OrderBy(x => x.Name).OrderBy(x => x.Time);



                dataToSendSorted.WithDeepEqual(dataBackSorted).Assert();

            }
            finally
            {
                repo.DropDB(TestDB);
            }


        }


        public IList<MachineIsRunningInflux> GenerateMachineIsRunningData(string department, (string Line, string Name) machine, (int Hour, int Minutes) ShiftStart, (int Hour, int Minutes) ShiftEnd, int intervales)
        {
            var listOfIsRunningAll = new List<MachineIsRunningInflux>();

            var StartDateTime = DateTime.Now.Date.AddHours(ShiftStart.Hour).AddMinutes(ShiftStart.Minutes).ToUniversalTime();

            var EndDateTime = DateTime.Now.Date.AddHours(ShiftEnd.Hour).AddMinutes(ShiftEnd.Minutes).ToUniversalTime();

            TimeSpan timeRange = EndDateTime - StartDateTime;

            var currentTime = StartDateTime;

            bool lastIsRunning = false;

            TimeSpan intervalLength = timeRange / intervales;

            while (currentTime < EndDateTime)
            {
                var newMachine = new MachineIsRunningInflux();

                newMachine.Department = department;
                newMachine.Line = machine.Line;
                newMachine.Name = machine.Name;
                newMachine.Time = currentTime;
                newMachine.IsRunning = !lastIsRunning;

                listOfIsRunningAll.Add(newMachine);

                lastIsRunning = !lastIsRunning;
                currentTime = currentTime.AddSeconds(intervalLength.TotalSeconds);
            }
            //listOfIsRunningAll.AddRange(listOfMachineIsRunning);
            //}

            return listOfIsRunningAll;
        }


        public IList<MachineIsRunningInflux> GenerateRandomIsRunningData(string department, (string Line, string Name)[] machines, (int Hour, int Minutes) ShiftStart, (int Hour, int Minutes) ShiftEnd)
        {
            var listOfIsRunningAll = new List<MachineIsRunningInflux>();

            var StartDateTime = DateTime.Now.Date.AddHours(ShiftStart.Hour - 1).AddMinutes(ShiftStart.Minutes);

            var EndDateTime = DateTime.Now.Date.AddHours(ShiftEnd.Hour + 1).AddMinutes(ShiftEnd.Minutes);

            Random rand = new Random();

            foreach (var machine in machines)
            {
                var currentTime = StartDateTime.ToUniversalTime();

                bool lastIsRunning = false;

                IList<MachineIsRunningInflux> listOfMachineIsRunning = new List<MachineIsRunningInflux>();

                while (currentTime < EndDateTime.ToUniversalTime())
                {
                    var newMachine = new MachineIsRunningInflux();

                    newMachine.Department = department;
                    newMachine.Line = machine.Line;
                    newMachine.Name = machine.Name;
                    newMachine.Time = currentTime;
                    newMachine.IsRunning = !lastIsRunning;

                    listOfMachineIsRunning.Add(newMachine);

                    lastIsRunning = !lastIsRunning;
                    currentTime = currentTime.AddSeconds(rand.Next(15, 7200));

                }
                listOfIsRunningAll.AddRange(listOfMachineIsRunning);
            }

            return listOfIsRunningAll;
        }


    }
}
