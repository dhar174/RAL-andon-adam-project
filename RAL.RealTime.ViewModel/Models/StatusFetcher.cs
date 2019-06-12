using DynamicData;
using RAL.RealTime.ViewModels;
using RAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheColonel2688.Utilities;

namespace RAL.RealTime.Models
{
    public class StatusFetcher
    {
        public SourceList<MachineStatus> MachineStatuses;

        //[Reactive]
        // (string ipaddress, string username, string password, TimeSpan timeOut, string databaseName, int port) RepositorySettings { get; set; }

        private IMachineRepository _repository;

        public SourceList<MachineKey> SubscribedMachines;

        //private List<MachineKey> _subscribedMachines;

        private NonReentrantTimer _timer;

        public event EventHandler<EventArgs> OnStatusesUpdated;

        public StatusFetcher(IMachineRepository repository, TimeSpan pollTimeInterval, List<MachineKey> machineKeys)
        {
            _repository = repository;
            SubscribedMachines = new SourceList<MachineKey>();
            SubscribedMachines.AddRange(machineKeys);

            MachineStatuses = new SourceList<MachineStatus>();
            machineKeys.ForEach(x => 
            {
                    MachineStatuses.Add(new MachineStatus() { Line = x.Line, Name = x.Name, Status = Status.Unknown });
            });

            _timer = new NonReentrantTimer(pollTimeInterval.TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;            
            
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var machine in SubscribedMachines.Items)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var result = await _repository.MachineIsRunningRepo.LastOrDefaultAsync(machine.Line, machine.Name);

                        var status = MachineStatuses.Items.First(x => x.Line == machine.Line && x.Name == machine.Name);

                        if (result is null)
                        {
                            status.Status = Status.Unknown;
                            return;
                        }

                        if (result.IsRunning)
                        {
                            status.Status = Status.Running;
                        }
                        else
                        {
                            status.Status = Status.Idle;
                        }

                    }));
                }
                Task.WaitAll(tasks.ToArray());

                OnStatusesUpdated?.Invoke(this, new EventArgs());
            }
            catch (OperationCanceledException ex)
            {
                //**ignore
            }
        }
    }
}
