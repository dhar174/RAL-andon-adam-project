using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheColonel2688.Utilities
{
    public static class TimeOut
    {
        public static async Task<T> Execute<T>(Func<Task<T>> method, TimeSpan timeOut)
        {
            using (var ctsForTask = new CancellationTokenSource())
            {
                var taskTimeOut = Task.Delay(timeOut);
                Task<T> task = Task.Run(() => method(), ctsForTask.Token);
                if (task != await Task.WhenAny(task, taskTimeOut))
                {
                    ctsForTask.Cancel();
                    throw new OperationCanceledException(ctsForTask.Token);
                }
                return task.Result;
            }
        }


        public static async Task Execute(Func<Task> method, TimeSpan timeOut)
        {
            using (var ctsForTask = new CancellationTokenSource())
            {
                var taskTimeOut = Task.Delay(timeOut);
                Task task = Task.Run(() => method(), ctsForTask.Token);
                if (task != await Task.WhenAny(task, taskTimeOut))
                {
                    ctsForTask.Cancel();
                    throw new OperationCanceledException(ctsForTask.Token);
                }
            }
        }
    }
}
