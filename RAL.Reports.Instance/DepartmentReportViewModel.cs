using System.Collections.Generic;

namespace RAL.Reports
{ 
    public class DepartmentReportViewModel
    {
        public IList<MachineStatesForTimePeriod<bool>> MachineStates { get; set; }

        public string Title { get; set; }
    }
}
