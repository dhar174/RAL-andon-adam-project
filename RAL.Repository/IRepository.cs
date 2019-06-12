using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RAL.Repository
{
    public interface IRepository<T>
    {
        string MeasurementName { get; }

        Task<T> LastOrDefaultAsync(string line, string name);

        Task WriteAsync(T row);

        Task WriteAsync(IEnumerable<T> rows);

    }
}
