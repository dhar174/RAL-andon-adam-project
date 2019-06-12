using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Reports
{
    public interface IReportDaily : IReport
    {
        //string Name { get; }

        (int Hour, int Minute) StartTime { get; set; }
        (int Hour, int Minute) EndTime { get; set; }

        //void Execute();
    }
}
