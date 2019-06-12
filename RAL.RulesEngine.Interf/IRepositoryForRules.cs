using System;
using System.Threading.Tasks;

namespace RAL.Rules.Interfaces
{
    public interface IRepositoryForRules
    {
        Task<(string Line, string Name, DateTime Time, bool IsCycling)?> LastOrDefaultStatusWhereIsAsync(IMachineForRules machine, bool IsCycling);
        Task WriteAsync(IMachineForRules machine, DateTime time, bool IsRunning);
        Task<(string Line, string Name, DateTime Time, bool IsCycling)?> LastOrDefaultStatusAsync(string line, string name);
        Task<(string Line, string Name, DateTime Time, bool IsRunning)?> LastOrDefaultIsRunningAsync(string line, string name);
    }
}