using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.Manager.Configuration
{
    public class DatabaseConfiguration
    {

        public string DatabaseName { get; private set; }
        public string IPAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; set; }

        public DatabaseConfiguration(string iPAddress, string username, string password, string databaseName)
        {
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            IPAddress = iPAddress ?? throw new ArgumentNullException(nameof(iPAddress));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
