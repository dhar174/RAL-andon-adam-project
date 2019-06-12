using RAL.RealTime.Models;
using RAL.Repository;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RAL.RealTime.ViewModels
{
    public class MachineLayoutViewModel : ReactiveObject
    {
        private ObservableAsPropertyHelper<IEnumerable<MachineStatusViewModel>> _machineStatuses;
        public IEnumerable<MachineStatusViewModel> MachineStatuses => _machineStatuses.Value;

        private readonly StatusFetcher statusFetcher;

        public MachineLayoutViewModel()
        {
            var repository = new MachineRepository("172.26.28.250", "RALSystem","1234", TimeSpan.FromSeconds(20));

            var MachinesToSubscribeTo = new List<MachineKey>() { new MachineKey("F-7-3", "Press"), new MachineKey("F-4-5", "Press") };

            statusFetcher = new StatusFetcher(repository: repository, TimeSpan.FromMilliseconds(1000), MachinesToSubscribeTo);

            
           
        }



    }
}
