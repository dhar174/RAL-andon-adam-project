using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Reports
{
    public interface IReport
    {
        string Name { get; }

        //** TODO these need to be removed this is a stop gap measure until I can come up with a better fluent instantiation mechanism
        //ILogger Logger { get; set; }
        //IRepositoryForReports Repository { get; set; }
        //void Validate();
        //** End Need Removed

        void Execute();
    }
}
