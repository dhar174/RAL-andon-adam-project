using FluentScheduler;

namespace RAL.Reports.Scheduler
{
    public class ReportJob : IJob
    {
        public IReportDaily Report { get; private set; }

        public ReportJob(IReportDaily report)
        {
            Report = report;
        }

        public void Execute()
        {
             Report.Execute();
        }
    }
}
