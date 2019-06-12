using RAL.RealTime.ViewModels;
using ReactiveUI.Fody.Helpers;

namespace RAL.RealTime.Models
{
    public class MachineStatus
    {
        [Reactive]
        public string Name { get; set; }
        [Reactive]
        public string Line { get; set; }
        [Reactive]
        public Status Status { get; set; }

    }
}