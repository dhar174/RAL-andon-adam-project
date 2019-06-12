using RAL.Repository;
using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RAL.Reports
{
    public interface IRepositoryForReports
    {
        Task<IList<MachineIsRunningInflux>> GetDataForShiftReportAsync(DateTime start, DateTime end, string department);
    }
}
