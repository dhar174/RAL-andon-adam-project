using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheColonel2688.Utilities;
using System.IO;
using RazorLight;
using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using FluentEmail.Core.Models;
using RAL.Reports.Base;
using System.Runtime.Serialization;
using RAL.Repository;
using RAL.Repository.Model;

namespace RAL.Reports
{
    public class ReportException : Exception
    {
        public ReportException()
        {
        }

        public ReportException(string message) : base(message)
        {
        }

        public ReportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }



    public class DepartmentTimePeriodReportForCurrentDay : IReportDaily
    {

        public ILogger Logger { get; set; }

        public IMachineRepository Repository { get; set; }


        private string _department;

        public List<Address> EmailAddresses { get; private set; }

        private IFluentEmail _emailSender;

        private string _pathToTemplate;

        public (int Hour, int Minute) StartTime { get; set; }
        public (int Hour, int Minute) EndTime { get; set; }

        public string Name { get; set; }

        private RazorLightEngine engine;

        public DepartmentTimePeriodReportForCurrentDay(string from, string[] emailAddresses, string department, string smtpServerHostName, int smtpServerPort, (string UserName, string Password)? smtpServerCredentials, string pathToTemplate, IMachineRepository repository, ILogger logger = null)
        {
           engine = new RazorLightEngineBuilder()
                      .UseFilesystemProject(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                      .UseMemoryCachingProvider()
                      .Build();

            /*if(pathToTemplate is null)
            {
                pathToTemplate = $"ReportTemplates{Path.DirectorySeparatorChar}DepartmentReportView.cshtml";
            }*/

            EmailAddresses = new List<Address>();

            foreach (var address in emailAddresses)
            {
                EmailAddresses.Add(new Address(address));
            }

            _emailSender = Email.From(from, "TRMI RAL System");

            var smtpClient = new SmtpClient(smtpServerHostName, smtpServerPort);
            if(!(smtpServerCredentials is null))
            {
                smtpClient.Credentials = new NetworkCredential(smtpServerCredentials.Value.UserName, smtpServerCredentials.Value.Password);
            }
            

            _emailSender.Sender = new SmtpSender(smtpClient);

            _department = department;

            var tempTemplatePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}{pathToTemplate}";

            if (!File.Exists(tempTemplatePath))
            {
                throw new FileNotFoundException($"Email Template File [{tempTemplatePath}] Not Found", pathToTemplate);
            }
          
            _pathToTemplate = tempTemplatePath;
            Logger = logger;
            Repository = repository;


        }

        public async Task<IEnumerable<MachineStatesForTimePeriod<bool>>> GetReportData()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            (DateTime startDateTime, DateTime endDateTime) = DateTimeHelper.GetStartAndEndDateTimeUsingEndDate(StartTime, EndTime, DateTime.Now.Date);

            IList<MachineInfoInflux> listOfMachines;
            try
            {
                listOfMachines = await Repository.MachineIsRunningRepo.GetAllMachinesWithRecordsAsync(_department);
            }
            catch (Exception)
            {
                throw;
            }

            IList<MachineIsRunningInflux> result;
            //** Get Data from Database
            try
            {
                result = await Repository.MachineIsRunningRepo.GetDataForTimeRangeReportAsync(
                    startDateTime.ToUniversalTime(),
                    endDateTime.ToUniversalTime(),
                    _department);
            }
            catch (Exception)
            {
                throw;
            }

            if(result is null)
            {
                throw new ReportException($"Query from Database returned null.");
            }



            IOrderedEnumerable<((string Line, string Name) Machine, List<(DateTime Time, bool IsRunning)> States)> resultsGroupedByMachine = result.GroupBy(
                m => new { m.Line, m.Name },
                m => (Time: m.Time.ToLocalTime(), m.IsRunning),
                (key, g) => (Machine:(key.Line, key.Name), States:g.ToList())).OrderBy(x => x.Machine.Line);


            //** check if there any missing machines

            List<MachineStatesForTimePeriod<bool>> AllTransformedMachines = new List<MachineStatesForTimePeriod<bool>>();

            //** Code block to contain tasks
            {
                //var tasks = new List<Task>();

                foreach (var machine in listOfMachines)
                {
                    //tasks.Add(Task.Run(async () =>
                    {
                        var numberOfEntries = resultsGroupedByMachine.Count(x => x.Machine.Line == machine.Line && x.Machine.Name == machine.Name);
                        if (numberOfEntries == 0)
                        {
                            var lastFromDB = await Repository.MachineIsRunningRepo.LastOrDefaultAsync(machine.Line, machine.Name);
                            //lastFromDB.Time = lastFromDB.Time.ToLocalTime();

                            var MachineStateOverTime = new MachineStatesForTimePeriod<bool>()
                            {
                                MachineInfo = (machine.Line, machine.Name),
                                States = new List<StateOverTime<bool>>()
                                {
                                    new StateOverTime<bool>()
                                    {
                                        Start = startDateTime,
                                        End = endDateTime,
                                        State = lastFromDB.IsRunning
                                    }
                                }
                            };

                            AllTransformedMachines.Add(MachineStateOverTime);
                        }
                    }
                }

            }



            foreach (var machine in resultsGroupedByMachine)
            {

                var LastStateAsInflux = await Repository.MachineIsRunningRepo.LastOrDefaultBeforeAsync(machine.Machine.Line, machine.Machine.Name, startDateTime);

                var lastState = (time: DateTime.MinValue, State: false);
                lastState = LastStateAsInflux is null ? lastState : (LastStateAsInflux.Time, LastStateAsInflux.IsRunning);
                lastState.time = lastState.time.ToLocalTime();

                IList<StateOverTime<bool>> states = MachineStateTransformer.Transform(startDateTime, endDateTime, machine.States, lastState.State);
            
                AllTransformedMachines.Add(new MachineStatesForTimePeriod<bool>() { MachineInfo = (machine.Machine.Line, machine.Machine.Name), States = states });
            }

            stopWatch.Stop();
            Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Debug("Method Execution took {elapsed}", stopWatch.Elapsed);
            return AllTransformedMachines;
        }


        public async Task<string> GenerateReportPresentationAsync(IEnumerable<MachineStatesForTimePeriod<bool>> data)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var viewmodel = new DepartmentReportViewModel
            {
                MachineStates = data.ToList(),
                Title = Name
            };

            //** If you receive an exception about missing meta data then you need to add 
            //**<PreserveCompilationContext>true</PreserveCompilationContext> to the.csproj of the STARTUP project
            string reportAsHTML = await engine.CompileRenderAsync(_pathToTemplate, viewmodel);

            stopWatch.Stop();
            Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Debug("Method Execution took {elapsed}", stopWatch.Elapsed);
            return reportAsHTML;
        }

        public void Send(string emailBodyAsHTML)
        {
            _emailSender
                .To(EmailAddresses)
                .Subject(Name)
                .Body(emailBodyAsHTML, true).Send();
        }

        public void Validate()
        {
            if(Repository is null)
            {
                throw new InvalidOperationException($"{nameof(Repository)} cannot be null, please assign it.");
            }
        }

        public void Execute()
        {
            try
            {
                Logger.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Information("Report Task Starting...");
                Validate();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Information("Report Task is Starting");
                var ReportDataTask = GetReportData().Result;
                //ReportDataTask.Wait();

                var ReportData = ReportDataTask;

                var reportAsHTML = GenerateReportPresentationAsync(ReportData).Result;

                Send(reportAsHTML);

                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Information("Report Task is Completed");
                stopWatch.Stop();
                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Information("Method Execution took {elapsed}", stopWatch.Elapsed);
            }
            catch (Exception ex) when(ex.Message.Contains("Query from Database returned null."))
            {
                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Error(ex, "Query from Database returned null.");
            }
            catch (FileNotFoundException ex)
            {
                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Error(ex, "File Not Found");
            }
            catch (Exception ex)
            {
                Logger?.Here(nameof(DepartmentTimePeriodReportForCurrentDay), Name).Error(ex, "Unknown Exception was thrown. Ignoring.");
            }

            

        }
    }
}
