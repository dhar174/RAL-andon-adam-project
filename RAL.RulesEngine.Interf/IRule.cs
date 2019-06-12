using RAL.Rules.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace RAL.Rules
{
    public interface IRule
    {
        string Description { get; }
        //TimeSpan PollInterval { get; set; }

        //** TODO these need to be removed this is a stop gap measure until I can come up with a better fluent instantiation mechanism
        //ILogger Logger { get; set; }
        //IRepositoryForRules Repository { get; set; }
        //** End Need Removed

        void Close();
        Task CloseAsync();

        WaitHandle GetCloseWaitHandle();

        Task StartAsync();
        void Start();

        Task StopAsync();
        void Stop();

        void WaitForClose();
        void WaitForCloseAsync();
    }
}