using System;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{

    public class MachineRepository : MachineRepositoryBase
    {

        public MachineRepository(string ipaddress, string username, string password, TimeSpan timeOut, string databaseName = "TRMI_RAL_System_Dev", int port = 8086) : base(ipaddress, username, password, databaseName, timeOut, port)
        {
            var uri = new Uri($"http://{ipaddress}:{port}");

            MachineStatusRepo = new MachineStatusRepository(_client, databaseName);
            MachineIsConnectedRepo = new MachineIsConnectedRepo(_client, databaseName);

            //var _client2 = new InfluxClient(uri, "TRMI_Trends", "1234");
            //_client2.Timeout = TimeSpan.FromMilliseconds(3000);
            MachineIsRunningRepo = new MachineIsRunningRepository(_client, databaseName);
        }
    }
}
