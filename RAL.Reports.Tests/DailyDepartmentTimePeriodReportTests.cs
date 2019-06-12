using Xunit;
using Xunit.Abstractions;
using RAL.Repository;
using System.Linq;
using DeepEqual.Syntax;
using System.IO;
using RAL.Reports.Scheduler;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RAL.Reports.Base;
using TheColonel2688.Utilities;

namespace RAL.Reports.Tests
{



    public class DailyDepartmentTimePeriodReportTests
    {

        private const string databaseIP = "192.168.1.208";

        private readonly ITestOutputHelper output;

        public DailyDepartmentTimePeriodReportTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        //private ReportHelper reportHelper = new ReportHelper();





        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetShiftReportDataWithMissingMachines(bool InitialState)
        {


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GetReportData)}";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);


            (int Hour, int Minute) ShiftStart = (07, 00);
            (int Hour, int Minute) ShiftEnd = (15, 00);

            var testDepartment = "TestDepartment";

            (var ExpectedReportDataSorted, var ToWriteDataSorted) = ReportTestHelper.GenerateReportTestDataWithMissing(InitialState, testDepartment, ShiftStart, ShiftEnd);

            output.WriteLine($"Expected Data");
            foreach (var machine in ExpectedReportDataSorted)
            {
                output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                foreach (var state in machine.States)
                {
                    output.WriteLine($"\t{state}");
                }
            }

            output.WriteLine($"To Write Data");
            foreach (var machine in ToWriteDataSorted)
            {
                output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                foreach (var state in machine.States)
                {
                    output.WriteLine($"\t{state}");
                }
            }

            Assert.True(ExpectedReportDataSorted.Count() == ToWriteDataSorted.Count());

            for (int i = 0; i < ExpectedReportDataSorted.Count(); i++)
            {
                Assert.Equal(ExpectedReportDataSorted[i].States.Count, ToWriteDataSorted[i].States.Count);

                for (int a = 0; a < ExpectedReportDataSorted[i].States.Count; a++)
                {
                    Assert.Equal(ExpectedReportDataSorted[i].States[a].State, ToWriteDataSorted[i].States[a].State);
                    Assert.Equal(ExpectedReportDataSorted[i].States[a].TimeSpan.Milliseconds, ToWriteDataSorted[i].States[a].TimeSpan.Milliseconds);
                }
            }

            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ToWriteDataSorted.ToArray(), null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            try
            {

                var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, 
                    testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
                {
                    StartTime = ShiftStart,
                    EndTime = ShiftEnd,
                    Name = "1st Shift Report (Test)"
                };

                var returnedResults = ShiftReport.GetReportData().Result;

                var ReturnedResultSorted = returnedResults.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);

                foreach (var machine in ReturnedResultSorted)
                {
                    machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x => x.State).ToList();
                }

                output.WriteLine($"Date has been returned and sorted");

                //** Output data to "Console"
                foreach (var machine in ReturnedResultSorted)
                {
                    output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                    foreach (var state in machine.States)
                    {
                        output.WriteLine($"\t{state}");
                    }
                }

                ReturnedResultSorted.ShouldDeepEqual(ExpectedReportDataSorted);

            }
            finally
            {
                repo.DropDB(TestDB);
            }
        }


        [Theory]
        
        [InlineData(true,   new int[] { 07, 00 },   new int[] { 08, 00 },  new int[] { 01, 00, 00, 000 })]
        [InlineData(false,  new int[] { 07, 00 },   new int[] { 08, 00 },  new int[] { 01, 00, 00, 000 })]

        [InlineData(true,   new int[] { 07, 26 },   new int[] { 08, 26 },  new int[] { 01, 00, 00, 000 })]
        [InlineData(false,  new int[] { 07, 26 },   new int[] { 08, 26 },  new int[] { 01, 00, 00, 000 })]

        [InlineData(true,   new int[] { 07, 26 },   new int[] { 15, 26 },  new int[] { 01, 00, 00, 000 })]
        [InlineData(false,  new int[] { 07, 26 },   new int[] { 15, 26 },  new int[] { 01, 00, 00, 000 })]

        [InlineData(true, new int[] { 22, 30 }, new int[] { 06, 30 }, new int[] { 00, 00, 00, 000 })]
        [InlineData(false, new int[] { 22, 30 }, new int[] { 06, 30 }, new int[] { 00, 00, 00, 000 })]
        
        [InlineData(true,   new int[] { 07, 26 },   new int[] { 07, 25 },  new int[] { 01, 00, 00, 000 })]
        [InlineData(false,  new int[] { 07, 26 },   new int[] { 07, 25 },  new int[] { 01, 00, 00, 000 })]

        //** TODO This Test need cleaned up immensely

        public void GetTimePeriodReportDataWithMissingMachines(bool InitialState, int[] ReportTimePeriodStartAsArray, int [] ReportTimePeriodEndAsArray, int[] marginAsArray)
        {
            TimeSpan margin = TimeSpan.FromHours(marginAsArray[0]) + TimeSpan.FromMinutes(marginAsArray[1]) + TimeSpan.FromSeconds(marginAsArray[2]) + TimeSpan.FromMilliseconds(marginAsArray[3]);
            (int Hour, int Minute) ReportTimePeriodStart = (ReportTimePeriodStartAsArray[0], ReportTimePeriodStartAsArray[1]);
            (int Hour, int Minute) ReportTimePeriodEnd = (ReportTimePeriodEndAsArray[0], ReportTimePeriodEndAsArray[1]);

            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GetReportData)}";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

            var testDepartment = "TestDepartment";

            (var ExpectedReportDataSorted, var ToWriteDataSorted) = ReportTestHelper.GenerateReportTestDataWithMissing(InitialState, testDepartment, ReportTimePeriodStart, ReportTimePeriodEnd, margin);
            
            //** Validate generated Data, yes, I am testing part of my test, this is smelly and need to be fixed. 
            Assert.True(ExpectedReportDataSorted.Count() == ToWriteDataSorted.Count());

            for (int i = 0; i < ExpectedReportDataSorted.Count(); i++)
            {
                Assert.Equal(ExpectedReportDataSorted[i].States.Count, ToWriteDataSorted[i].States.Count);

                for (int a = 0; a < ExpectedReportDataSorted[i].States.Count; a++)
                {
                    Assert.Equal(ExpectedReportDataSorted[i].States[a].State, ToWriteDataSorted[i].States[a].State);
                }
            }

            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ToWriteDataSorted.ToArray(), null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            try
            {

                var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" },
                    testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
                {
                    StartTime = ReportTimePeriodStart,
                    EndTime = ReportTimePeriodEnd,
                    Name = "1st Shift Report (Test)"
                };

                var returnedResults = ShiftReport.GetReportData().Result;

                var ReturnedResultSorted = returnedResults.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);

                foreach (var machine in ReturnedResultSorted)
                {
                    machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x => x.State).ToList();
                }

                output.WriteLine($"Date has been returned and sorted");

                //** Output data to "Console"
                foreach (var machine in ReturnedResultSorted)
                {
                    output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                    foreach (var state in machine.States)
                    {
                        output.WriteLine($"\t{state}");
                    }
                }
                try
                {
                    ReturnedResultSorted.ShouldDeepEqual(ExpectedReportDataSorted);
                }
                catch(Exception ex)
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string FilePath = $"{desktopPath}{Path.DirectorySeparatorChar}{nameof(GetTimePeriodReportDataWithMissingMachines)}Failure{DateTime.Now.Ticks}";
                    ObjectDifferenceWriter.DumpToFile(
                        ReturnedResultSorted.WithDeepEqual(ExpectedReportDataSorted).Expected, 
                        ReturnedResultSorted.WithDeepEqual(ExpectedReportDataSorted).Actual,
                        FilePath
                        );
                    throw;
                }

            }
            finally
            {
                repo.DropDB(TestDB);
            }
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetReportData(bool InitialState)
        {

            var ShiftStart = (Hours: 07, Minutes: 00);
            var ShiftEnd = (Hours: 15, Minutes: 00);


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GetReportData)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }
            
            repo.CreateDB(TestDB);

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


            var ExpectedReportData = ReportTestHelper.GenerateMachineIsRunningDataForExpectedMulti(testDepartment, listOfMachines, ShiftStart, ShiftEnd, InitialState);


            var ExpectedReportDataSorted = ExpectedReportData.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);

            output.WriteLine($"Out Going Data");
            foreach (var machine in ExpectedReportDataSorted)
            {
                output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                foreach (var state in machine.States)
                {
                    output.WriteLine($"\t{state}");
                }
            }

            foreach (var machine in ExpectedReportDataSorted)
            {
                machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x=> x.State).ToList();
            }

            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ExpectedReportData, null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            try
            {

                var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, testDepartment, 
                    "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
                {
                    StartTime = ShiftStart,
                    EndTime = ShiftEnd,
                    Name = "1st Shift Report (Test)"
                };

                var returnedResults = ShiftReport.GetReportData().Result;

                var ReturnedResultSorted = returnedResults.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);

                foreach (var machine in ReturnedResultSorted)
                {
                    machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x => x.State).ToList();
                }

                output.WriteLine($"Date has been returned and sorted");

                //** Output data to "Console"
                foreach (var machine in ReturnedResultSorted)
                {
                    output.WriteLine($"{machine.MachineInfo.Line}.{machine.MachineInfo.Name}");
                    foreach (var state in machine.States)
                    {
                        output.WriteLine($"\t{state}");
                    }
                }

                ReturnedResultSorted.WithDeepEqual(ExpectedReportDataSorted).Assert();

            }
            finally
            {
                repo.DropDB(TestDB);
            }
        }

        [Fact]
        public void GenerateReportPresentation()
        {
            var ShiftStartTime = (Hours: 07, Minutes: 00);
            var ShiftEndTime = (Hours: 15, Minutes: 00);


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GenerateReportPresentation)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

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
                ("W-2-1", Name: "Press"),
                ("W-2-1", Name: "Press")
            };

            var ExpectedReportData = ReportTestHelper.GenerateMachineIsRunningDataForExpectedMulti(testDepartment, listOfMachines, ShiftStartTime, ShiftEndTime);

            var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new []{"bnewman@betzmachine.com"}, testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo, null);

            string path = System.Reflection.Assembly.GetAssembly(typeof(DepartmentTimePeriodReportForCurrentDay)).Location;

            var reportAsHTML = ShiftReport.GenerateReportPresentationAsync(ExpectedReportData).Result;

            //** TODO need a fixed set of Report Data and an Assert on the HTML

        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetDataAndSendUsingIPowerWithMissingData(bool InitialState)
        {
            var ShiftStartTime = (Hours: 07, Minutes: 00);
            var ShiftEndTime = (Hours: 15, Minutes: 00);


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GenerateReportPresentation)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository("192.168.1.208", "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

            (var ExpectedReportDataSorted, var ToWriteDataSorted) = ReportTestHelper.GenerateReportTestDataWithMissing(InitialState, testDepartment, ShiftStartTime, ShiftEndTime);
            
            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ToWriteDataSorted.ToArray(), null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, testDepartment,
                "smtp.ipower.com", 587, ("betz@betzmachine.com", "Betz-320"), $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                StartTime = ShiftStartTime,
                EndTime = ShiftEndTime,
                Name = "1st Shift Test Report (Dummy Data)"
            };

            var returnedResults = ShiftReport.GetReportData().Result;

            var reportAsHTML = ShiftReport.GenerateReportPresentationAsync(returnedResults).Result;

            ShiftReport.Send(reportAsHTML);
        }

        /*
        [Fact]
        public void SendUsingIPower()
        {
            var ShiftStartTime = (Hours: 07, Minutes: 00);
            var ShiftEndTime = (Hours: 15, Minutes: 00);


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GenerateReportPresentation)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository("192.168.1.208", "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

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
                ("W-2-1", Name: "Press"),
                ("W-2-1", Name: "Press")
            };

            var ExpectedReportData = ReportTestHelper.GenerateMachineIsRunningDataForExpectedMulti(testDepartment, listOfMachines, ShiftStartTime, ShiftEndTime);

            var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com",new[] { "bnewman@betzmachine.com" }, testDepartment, 
                "smtp.ipower.com", 587, ("betz@betzmachine.com", "Betz-320"), $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                Name = "1st Shift Test Report (Dummy Data)"
            };

            string path = System.Reflection.Assembly.GetAssembly(typeof(DepartmentTimePeriodReportForCurrentDay)).Location;

            var reportAsHTML = ShiftReport.GenerateReportPresentationAsync(ExpectedReportData).Result;

            ShiftReport.Send(reportAsHTML);
        }
        */
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SendUsingSlurperWithMissingData(bool InitialState)
        {
            var ShiftStartTime = (Hours: 07, Minutes: 00);
            var ShiftEndTime = (Hours: 15, Minutes: 00);


            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(GenerateReportPresentation)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

            (var ExpectedReportDataSorted, var ToWriteDataSorted) = ReportTestHelper.GenerateReportTestDataWithMissing(InitialState, testDepartment, ShiftStartTime, ShiftEndTime);

            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ToWriteDataSorted, null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            var ShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" },
                testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                Name = "1st Shift Test",
                StartTime = ShiftStartTime,
                EndTime = ShiftEndTime,
            };

            string path = System.Reflection.Assembly.GetAssembly(typeof(DepartmentTimePeriodReportForCurrentDay)).Location;

            var returnedResults = ShiftReport.GetReportData().Result;

            var reportAsHTML = ShiftReport.GenerateReportPresentationAsync(returnedResults).Result;

            ShiftReport.Send(reportAsHTML);
        }

        [Fact]
        public void SendWithSlurperUsingScheduler()
        {
            var FirstShiftStartTime = (Hours: 07, Minutes: 00);
            var FirstShiftEndTime = (Hours: 09, Minutes: 00);

            var SecondShiftStartTime = (Hours: 09, Minutes: 00);
            var SecondShiftEndTime = (Hours: 11, Minutes: 00);

            var ThirdShiftStartTime = (Hours: 11, Minutes: 00);
            var ThirdShiftEndTime = (Hours: 13, Minutes: 00);

            var TestDB = $"{nameof(DailyDepartmentTimePeriodReportTests)}.{nameof(SendWithSlurperUsingScheduler)}";

            var testDepartment = "TestDepartment";

            var repo = new MachineRepository(databaseIP, "TestRunner", "1234", TimeSpan.FromMilliseconds(5000), databaseName: TestDB);

            repo.ThrowIfCantConnect();

            if (repo.DoesDBExist(TestDB))
            {
                repo.DropDB(TestDB);
            }

            repo.CreateDB(TestDB);

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
                ("W-2-1", Name: "Press"),
                ("W-2-1", Name: "Press")
            };

            var ExpectedReportData = ReportTestHelper.GenerateMachineIsRunningDataForExpectedMulti(testDepartment, listOfMachines, FirstShiftStartTime, ThirdShiftEndTime);

            var toSendData = ReportTestHelper.ConvertExpectedToTestInputDataMulti(testDepartment, ExpectedReportData, null);

            repo.MachineIsRunningRepo.WriteAsync(toSendData).Wait();

            var FirstShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, 
                testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                Name = "1st Shift 2 Hour Range Test",
                StartTime = FirstShiftStartTime,
                EndTime = FirstShiftEndTime
            };

            var SecondShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, 
                testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                Name = "2nd Shift 2 Hour Range Test",
                StartTime = SecondShiftStartTime,
                EndTime = SecondShiftEndTime
            };

            var ThirdShiftReport = new DepartmentTimePeriodReportForCurrentDay("bnewman@betzmachine.com", new[] { "bnewman@betzmachine.com" }, 
                testDepartment, "localhost", 2500, null, $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml", repo)
            {
                Name = "3rd Shift 2 Hour Range Test",
                StartTime = ThirdShiftStartTime,
                EndTime = ThirdShiftEndTime
            };

            var reportManager = new ReportsManager();
            reportManager.AddReportToRunAt(FirstShiftReport, (s) => s.ToRunEvery(0).Days().At(DateTime.Now.Hour, DateTime.Now.Minute + 1));
            reportManager.AddReportToRunAt(SecondShiftReport, (s) => s.ToRunEvery(0).Days().At(DateTime.Now.Hour, DateTime.Now.Minute + 1));
            reportManager.AddReportToRunAt(ThirdShiftReport, (s) => s.ToRunEvery(0).Days().At(DateTime.Now.Hour, DateTime.Now.Minute + 1));



            Task.Delay(TimeSpan.FromMinutes(3)).Wait();

        }
    }
}
