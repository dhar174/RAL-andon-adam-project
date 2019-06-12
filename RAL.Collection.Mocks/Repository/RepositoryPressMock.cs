using RAL.Repository;
using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Collector.Mocks.Repository
{
    public class RepositoryWriterMachineMock : IRepositoryWriterMachine
    {
        public delegate void ConnectionStatusAddDoer(IMachineInfo machine, bool IsConnected);

        public delegate DateTime StatusAddDoer(IMachineInfo machine, bool IsCycling);

        private ConnectionStatusAddDoer _connectionAddDoer;

        private StatusAddDoer _statusAddDoer;


        public RepositoryWriterMachineMock(ConnectionStatusAddDoer connectionStatusAddDoer, StatusAddDoer statusAddDoer)
        {
            _connectionAddDoer = connectionStatusAddDoer;
            _statusAddDoer = statusAddDoer;
        }


        public void AddConnectionStatusBegin(IMachineInfo machine, bool IsConnected)
        {
            _connectionAddDoer.Invoke(machine, IsConnected);
        }

        public DateTime AddStatusBegin(IMachineInfo machine, bool IsCycling)
        {
            return _statusAddDoer.Invoke(machine, IsCycling);
        }
    }
}
