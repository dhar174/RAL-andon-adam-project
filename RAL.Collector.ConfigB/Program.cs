
using RAL.Collector.Mocks.Converter;
using RAL.ConfigStorageTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ExtendedXmlSerializer.Configuration;
using RAL.Collector;
using RAL.RulesEngine.StackLights;
using RAL.RulesEngine;
using RAL.Repository;
using RAL.Reports;

namespace RAL.ConfigFileBootStrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}";
            string repositoryConfigPath = $"{rootPath}{Path.DirectorySeparatorChar}repository.xml.config";
            string machineSubscriptionsPath = $"{rootPath}{Path.DirectorySeparatorChar}machines.xml.config";
            string reportsConfigPath = $"{rootPath}{Path.DirectorySeparatorChar}reports.xml.config";

            WriteTestReposiotryConfig(repositoryConfigPath);
            WriteTestMachineConfig(machineSubscriptionsPath);
            WriteTestReportConfig(reportsConfigPath);

            Console.WriteLine("Done.");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void WriteTestReposiotryConfig(string repositoryConfigPath)
        {
            Console.WriteLine($"Writing Repository Config to {repositoryConfigPath}");

            var configMan = new RepositoryFactory
            {
                Config = new RepositoryConfigDSC()
                {
                    DatabaseIPaddress = "192.168.1.208",
                    DatabasePort = "8086",
                    DatabaseName = "TRMI_Production_Dev",

                }
            };

            configMan.SaveTo(repositoryConfigPath);
        }


        static void WriteTestMachineConfig(string machineSubscriptionsPath)
        {

            Console.WriteLine($"Writing Machine Config to {machineSubscriptionsPath}");

            Queue<(IStackLight5Light, StackLight5Lights.LightNumber)> lightQueue = new Queue<(IStackLight5Light, StackLight5Lights.LightNumber)>();

            SignaworksEthernetStackLight SigStackLight1 = new SignaworksEthernetStackLight("192.168.10.201");
            SignaworksEthernetStackLight SigStackLight2 = new SignaworksEthernetStackLight("192.168.10.202");
            SignaworksEthernetStackLight SigStackLight3 = new SignaworksEthernetStackLight("192.168.10.203");
            SignaworksEthernetStackLight SigStackLight4 = new SignaworksEthernetStackLight("192.168.10.204");

            var StackLight1 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight1);
            var StackLight2 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight2);
            var StackLight3 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight3);
            var StackLight4 = new SignaworksEthernetAsStackLightRYGBW(SigStackLight4);

            lightQueue.Enqueue((StackLight1, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight1, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight2, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight2, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight3, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight3, StackLight5Lights.LightNumber.Light1));
            lightQueue.Enqueue((StackLight4, StackLight5Lights.LightNumber.Light0));
            lightQueue.Enqueue((StackLight4, StackLight5Lights.LightNumber.Light1));

            var list = new List<MachineConfigDSC>();

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-1",
                Name = "Y-1-1",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051PayloadConverterStatus),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.201", null),
                        LightNumber = StackLight5Lights.LightNumber.Light0
                    }
                }

            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-2",
                Name = "Y-1-2",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock2),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.201", null),
                        LightNumber = StackLight5Lights.LightNumber.Light1
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-3",
                Name = "Y-1-3",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock3),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.202", null),
                        LightNumber = (int)StackLight5Lights.LightNumber.Light0
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-4",
                Name = "Y-1-4",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock4),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.202", null),
                        LightNumber = StackLight5Lights.LightNumber.Light1
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-5",
                Name = "Y-1-5",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock5),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.203", null),
                        LightNumber = StackLight5Lights.LightNumber.Light0
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-6",
                Name = "Y-1-6",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock6),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.203", null),
                        LightNumber = StackLight5Lights.LightNumber.Light1
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00D0C9FCA9BA",
                Line = "Y-1-7",
                Name = "Y-1-7",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock7),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.204", null),
                        LightNumber = StackLight5Lights.LightNumber.Light0
                    }
                }
            });

            list.Add(new MachineConfigDSC()
            {
                MAC = "00-D0-C9-FC-A9-BA",
                Line = "Y-1-8",
                Name = "Y-1-8",
                Department = "Parts Production",
                MachineType = typeof(PressAdam6051),
                PayloadConverterType = typeof(PressAdam6051MessageConverterMock8),
                Rules = new List<RuleIntervalDSC>()
                {
                    new RuleIntervalDSC() {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunning),
                        AllowedCycleTime = TimeSpan.FromSeconds(5)
                    },
                    new RuleIntervalDSC()
                    {
                        TypeOfRuleInterval = typeof(RuleMachineIsRunningStackLight),
                        StackLightUri = ("192.168.10.204", null),
                        LightNumber = StackLight5Lights.LightNumber.Light1
                    }
                }
            });


            var serializer = new ConfigurationContainer().Create();


            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter writer = XmlWriter.Create(machineSubscriptionsPath, settings))
            {
                serializer.Serialize(writer, list);
            }
        }


        static void WriteTestReportConfig(string reportsConfigPath)
        {

            Console.WriteLine($"Writing Reports Config to {reportsConfigPath}");
            var TestDepartment = "Test Department";

            string[] emailAddresses = { "bnewman@betzmachine.com" };
            (int Hours, int Minutes) report1stStartTime = (6, 30);
            (int Hours, int Minutes) report1stEndTime = (14, 30);
            (int Hours, int Minutes) report2ndStartTime = (12, 30);
            (int Hours, int Minutes) report2ndEndTime = (22, 30);

            var EmailReportsDSC = new EmailReportsDSC()
            {
                SMTPServerHostName = "localhost",
                SMTPServerPort = 2500,
                listOfReportConfigs = new EmailReportDSC[]
                    {
                        new EmailReportDSC() { Name = "1st Shift Down Time Report", Department  = TestDepartment, ReportTimeSpanStartTime = report1stStartTime, ReportTimeSpanEndTime = report1stEndTime, EmailAddress = emailAddresses, TypeOfEmailReport = typeof(DepartmentShiftReportForCurrentDay) },
                        new EmailReportDSC() { Name = "2nd Shift Down Time Report", Department  = TestDepartment, ReportTimeSpanStartTime = report2ndStartTime, ReportTimeSpanEndTime = report2ndEndTime, EmailAddress = emailAddresses, TypeOfEmailReport = typeof(DepartmentShiftReportForCurrentDay) }
                    }

            };

            var serializer = new ConfigurationContainer().Create();

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

            using (XmlWriter writer = XmlWriter.Create(reportsConfigPath, settings))
            {
                serializer.Serialize(writer, EmailReportsDSC);
            }
        }

    }
}
