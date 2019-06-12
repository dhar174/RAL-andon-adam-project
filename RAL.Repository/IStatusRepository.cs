using RAL.Repository.Model;
using System.Threading.Tasks;

namespace RAL.Repository
{
    public interface IMachineStatusRepository
    {
        Task<MachineStatusInflux> LastOrDefaultAsync(string line, string name);
        Task<MachineStatusInflux> LastOrDefaultWhereIsAsync(string line, string name, bool? isCycling = null, bool? isInAutomatic = null, bool? isFaulted = null);
        Task WriteAsync(MachineStatusInflux data);
    }
}