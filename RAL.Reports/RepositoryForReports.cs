using RAL.Repository;
using RAL.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RAL.Reports
{
    public class RepositoryForReports : IRepositoryForReports
    {

        private IMachineRepository _repository;

        public RepositoryForReports(IMachineRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<MachineIsRunningInflux>> GetDataForShiftReportAsync(DateTime start, DateTime end, string department)
        {
            return await _repository.MachineIsRunningRepo.GetDataForTimeRangeReportAsync(start, end, department);
        }


    }
}
