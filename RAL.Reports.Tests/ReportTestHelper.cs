using System;
using RAL.Repository;
using System.Collections.Generic;
using RAL.Repository.Model;
using System.Linq;
using RAL.Reports.Base;

namespace RAL.Reports.Tests
{
    public static class ReportTestHelper
    {
        public static (List<MachineStatesForTimePeriod<bool>> ExpectedReportData,
            List<MachineStatesForTimePeriod<bool>> ToWriteData)
        GenerateReportTestDataWithMissing(bool InitialState, string testDepartment, (int Hour, int Minute) ShiftStart, (int Hour, int Minute) ShiftEnd)
        {
            return GenerateReportTestDataWithMissing(InitialState, testDepartment, ShiftStart, ShiftEnd, TimeSpan.FromHours(1));
        }


        public static (List<MachineStatesForTimePeriod<bool>> ExpectedReportData,
            List<MachineStatesForTimePeriod<bool>> ToWriteData)
        GenerateReportTestDataWithMissing(bool InitialState, string testDepartment, (int Hour, int Minute) ShiftStart, (int Hour, int Minute) ShiftEnd, TimeSpan margin)
        {
            /*
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
            */

            //List<MachineStatesForTimePeriod<bool>> ExpectedReportData = GenerateMachineIsRunningDataForExpectedMulti(testDepartment, listOfMachines, ShiftStart, ShiftEnd, InitialState);

            List<MachineStatesForTimePeriod<bool>> ExpectedReportData = new List<MachineStatesForTimePeriod<bool>>();

            List<MachineStatesForTimePeriod<bool>> ToWriteData = new List<MachineStatesForTimePeriod<bool>>();

            //ToWriteData.AddRange(ExpectedReportData);


                var missingMachine1ToWrite = new MachineStatesForTimePeriod<bool>()
                {
                    MachineInfo = ("Missing Line 1", "Press"),

                    States = new List<StateOverTime<bool>>()
                    {
                        new StateOverTime<bool>()
                        {
                            Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime.Subtract(margin),
                            End =   DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime  .Add(margin),
                            State = InitialState
                        }
                    }
                };
            
            var missingMachine1Expected = new MachineStatesForTimePeriod<bool>()
            {
                MachineInfo = ("Missing Line 1", "Press"),
                States = new List<StateOverTime<bool>>()
                {
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime,
                        End =   DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime,
                        State = InitialState
                    }
                }
            };

            var missingMachine2ToWrite = new MachineStatesForTimePeriod<bool>()
            {
                MachineInfo = ("Missing Line 2", "Press"),
                States = new List<StateOverTime<bool>>()
                {
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime.Subtract(margin.Multiply(2)),
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime.Add(margin.Multiply(2)),
                        State = InitialState
                    }
                }
            };

            var missingMachine2Expected = new MachineStatesForTimePeriod<bool>()
            {
                MachineInfo = ("Missing Line 2", "Press"),
                States = new List<StateOverTime<bool>>()
                {
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime,
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime,
                        State = InitialState
                    }
                }
            };


            var blah3ToWrite = new MachineStatesForTimePeriod<bool>()
            {
                MachineInfo = ("Blah Line 3", "Press"),
                States = new List<StateOverTime<bool>>()
                {
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime.Subtract(margin),
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime.Add(margin.Multiply(0.25)),
                        State = InitialState
                    },
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime.Add(margin.Multiply(0.25)),
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime.Add(margin),
                        State = !InitialState
                    }
                }
            };

            var blah3Expected = new MachineStatesForTimePeriod<bool>()
            {
                MachineInfo = ("Blah Line 3", "Press"),
                States = new List<StateOverTime<bool>>()
                {
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime,
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime.Add(margin.Multiply(0.25)),
                        State = InitialState
                    },
                    new StateOverTime<bool>()
                    {
                        Start = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).StartDateTime.Add(margin.Multiply(0.25)),
                        End = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(ShiftStart, ShiftEnd, DateTime.Today).endDateTime,
                        State = !InitialState
                    }
                }
            };

            ExpectedReportData.Add(missingMachine1Expected);
            ExpectedReportData.Add(missingMachine2Expected);
            ExpectedReportData.Add(blah3Expected);
            ToWriteData.Add(missingMachine1ToWrite);
            ToWriteData.Add(missingMachine2ToWrite);
            ToWriteData.Add(blah3ToWrite);

            var ExpectedReportDataSorted = ExpectedReportData.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);

            var ToWriteDataSorted = ToWriteData.OrderBy(x => x.MachineInfo.Line).ThenBy(x => x.MachineInfo.Name);


            foreach (var machine in ExpectedReportDataSorted)
            {
                machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x => x.State).ToList();
            }

            foreach (var machine in ToWriteDataSorted)
            {
                machine.States = machine.States.OrderBy(x => x.Start).ThenBy(x => x.End).ThenBy(x => x.State).ToList();
            }



            return (ExpectedReportDataSorted.ToList(), ToWriteDataSorted.ToList());
        }


        public static List<MachineStatesForTimePeriod<bool>> GenerateMachineIsRunningDataForExpectedMulti(string department, (string Line, string Name)[] machines, (int Hour, int Minutes) ShiftStart, (int Hour, int Minutes) ShiftEnd, bool InitalState = false)
        {
            List<MachineStatesForTimePeriod<bool>> list = new List<MachineStatesForTimePeriod<bool>>();

            foreach (var machine in machines)
            {
                list.Add(GenerateMachineIsRunningDataForExpected(department, machine, ShiftStart, ShiftEnd));
            }

            return list;
        }

        public static MachineStatesForTimePeriod<bool> GenerateMachineIsRunningDataForExpected(string department, (string Line, string Name) machine, (int Hour, int Minutes) ShiftStart, (int Hour, int Minutes) ShiftEnd, bool InitalState = false)
        {
            var listOfStates = new List<StateOverTime<bool>>();


            var StartDateTime = DateTime.Now.Date.AddHours(ShiftStart.Hour).AddMinutes(ShiftStart.Minutes);

            var EndDateTime = DateTime.Now.Date.AddHours(ShiftEnd.Hour).AddMinutes(ShiftEnd.Minutes);

            Random rand = new Random();

            var currentStartTime = StartDateTime;

            bool lastIsRunning = InitalState;

            while (currentStartTime < EndDateTime)
            {
                var TempEndTime = currentStartTime.AddSeconds(rand.Next(15, 7200));

                DateTime currentEndTime;

                if (TempEndTime < EndDateTime)
                {
                    currentEndTime = TempEndTime;
                }
                else
                {
                    currentEndTime = EndDateTime;
                }

                var newState = new StateOverTime<bool>
                {
                    Start = currentStartTime,
                    End = currentEndTime,
                    State = !lastIsRunning
                };

                listOfStates.Add(newState);

                lastIsRunning = !lastIsRunning;

                currentStartTime = currentEndTime;
            }

            var newMachineStatesForShift = new MachineStatesForTimePeriod<bool>() { States = listOfStates, MachineInfo = machine };
            return newMachineStatesForShift;
        }

        public static DateTime GetRandomTimeBetween(DateTime Start, DateTime End, Random random = null)
        {
            if (random is null)
            {
                random = new Random();
            }

            TimeSpan timeSpan = End - Start;

            TimeSpan newSpan = TimeSpan.FromSeconds(random.Next(0, (int)timeSpan.TotalSeconds));
            DateTime newDate = Start + newSpan;

            return newDate;
        }


        public static List<MachineIsRunningInflux> ConvertExpectedToTestInputDataMulti(string department, IList<MachineStatesForTimePeriod<bool>> listOfShiftStatesForMachine, Random random = null)
        {
            var list = new List<MachineIsRunningInflux>();

            foreach (var shiftStatesForMachine in listOfShiftStatesForMachine)
            {
                list.AddRange(ConvertExpectedToTestInputData(department, shiftStatesForMachine, random));
            }

            return list;
        }


        public static List<MachineIsRunningInflux> ConvertExpectedToTestInputData(string department, MachineStatesForTimePeriod<bool> shiftStatesForMachine, Random random = null)
        {
            var listOfIsRunningAll = new List<MachineIsRunningInflux>();

            foreach (var state in shiftStatesForMachine.States)
            {
                {
                    var newMachineIsRunningInflux = new MachineIsRunningInflux
                    {
                        Department = department,
                        Line = shiftStatesForMachine.MachineInfo.Line,
                        Name = shiftStatesForMachine.MachineInfo.Name,
                        Time = state.Start.ToUniversalTime(),
                        IsRunning = state.State
                    };
                    listOfIsRunningAll.Add(newMachineIsRunningInflux);
                }

                if (!(random is null))
                {
                    var newMachineIsRunningInflux = new MachineIsRunningInflux
                    {
                        Department = department,
                        Line = shiftStatesForMachine.MachineInfo.Line,
                        Name = shiftStatesForMachine.MachineInfo.Line,
                        Time = GetRandomTimeBetween(state.Start.ToUniversalTime(), state.End.ToUniversalTime(), random),
                        IsRunning = state.State
                    };
                    listOfIsRunningAll.Add(newMachineIsRunningInflux);
                }

            }

            return listOfIsRunningAll;
        }


    }
}
