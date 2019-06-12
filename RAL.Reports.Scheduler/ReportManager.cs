using FluentScheduler;
using Serilog;
using System;
using System.Collections.Generic;

namespace RAL.Reports.Scheduler
{
    public class ReportsManager
    {

        private IList<IReport> allReports = new List<IReport>();

        private IList<IReportDaily> dailyReports = new List<IReportDaily>();

        private ILogger _logger;

        public ReportsManager(ILogger _loggerForManager = null)
        {
            _logger = _loggerForManager;
        }

        public ReportsManager(IList<(IReport report, Action<Schedule> schedule)> reportsWithSchedule, ILogger _loggerForManager)
        {
            foreach (var reportWithScehdule in reportsWithSchedule)
            {
                AddReportToRunAt(reportWithScehdule.report, reportWithScehdule.schedule);
            }
        }

        
        public void Start()
        {
            JobManager.Start();
        }
        

        public void AddReportToRunAt(IReport report, Action<Schedule> schedule)
        {
            allReports.Add(report);

            IReportDaily dailyReport = report as IReportDaily;

            if (dailyReport != null)
            {
                dailyReports.Add(dailyReport);
            }

            JobManager.AddJob(() => report.Execute(), schedule);
            
        }

        /*
        public void AddReportToRunAt(IReport, Action<RunSpecifier> specifier)
        {
            var 
        }
        */
        /*
        public void AddDailyReports(IEnumerable<IReportDaily> reports)
        {
            foreach(var report in reports)
            {
                AddDailyReport(report);
            }
        }
        */

        /*
        public void WaitForCloseToComplete()
        {
            throw new NotImplementedException();
        }
        */
    }
}
