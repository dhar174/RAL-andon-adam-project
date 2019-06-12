using Serilog;
using System;

namespace RAL.Repository
{
    public class MachineRepositoryCache : MachineRepositoryBase
    {
        public MachineRepositoryCache(string ipaddress, string username, string password, TimeSpan timeOut, string databaseName = "TRMI_RAL_System_Dev",  int port = 8086, ILogger logger = null) 
            : base(ipaddress, username, password, databaseName, timeOut, port)
        {
            MachineIsConnectedRepo = new MachineIsConnectedRepo(_client, databaseName);

            MachineStatusRepo = new MachineStatusRepositoryWithCache(client: _client, databaseName: databaseName, cacheWriteInterval: TimeSpan.FromMilliseconds(5000), logger: logger);
            MachineIsRunningRepo = new MachineIsRunningRepositoryWithCache(client: _client, databaseName: databaseName, cacheWriteInterval: TimeSpan.FromMilliseconds(5000), logger: logger);
        }
    }
}
