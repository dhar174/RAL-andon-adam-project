using ReactiveUI;
using System;
using System.Drawing;

namespace RAL.RealTime.ViewModels
{

    public enum Status { Unknown, Running, Idle, Faulted, Manual }

    public class MachineStatusViewModel : ReactiveObject
    {
        public string Name;
        public Status Status;
        public Color TextColor;
        public (int row, int number) Location;

        public MachineStatusViewModel()
        {
            this.WhenAnyValue(vm => vm.Status).Subscribe(HandleStatusColorBehavior).Dispose();
        }

        private void HandleStatusColorBehavior(Status status)
        {
            switch (status)
            {
                case Status.Running:
                    TextColor = Color.Green;
                    break;
                case Status.Idle:
                    TextColor = Color.Orange;
                    break;
                case Status.Faulted:
                    TextColor = Color.Red;
                    break;
                case Status.Manual:
                    TextColor = Color.Blue;
                    break;
            }
        }
    }
}
