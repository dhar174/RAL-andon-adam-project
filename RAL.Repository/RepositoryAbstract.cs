using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RAL.Repository.Model;
using Vibrant.InfluxDB.Client;

namespace RAL.Repository
{
    public abstract class RepositoryAbstract<T> : IRepository<T>
    {
        protected InfluxClient _client;
        protected string _databaseName;

        public string MeasurementName { get; protected set; }

        public RepositoryAbstract(InfluxClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
        }

        public abstract Task WriteAsync(T data);

        public abstract Task WriteAsync(IEnumerable<T> isRunnings);

        public abstract Task<T> LastOrDefaultAsync(string line, string name);

    }
}
