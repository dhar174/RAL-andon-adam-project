using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAL.Repository
{
    public interface IMachineIsRunningRepository
    {
        Task<IList<MachineIsRunningInflux>> GetDataForTimeRangeReportAsync(DateTime start, DateTime end, string department);
        Task<IList<MachineInfoInflux>> GetAllMachinesWithRecordsAsync(string department);
        Task<IList<MachineIsRunningInflux>> LastNOrDefaultAsync(int number, string line, string name);
        Task<IList<MachineIsRunningInflux>> LastNOrDefaultBeforeAsync(int number, string line, string name, DateTime before);
        Task<MachineIsRunningInflux> LastOrDefaultAsync(string line, string name);
        Task<MachineIsRunningInflux> LastOrDefaultBeforeAsync(string line, string name, DateTime before);
        Task WriteAsync(IEnumerable<MachineIsRunningInflux> isRunnings);
        Task WriteAsync(MachineIsRunningInflux isRunning);
    }
}