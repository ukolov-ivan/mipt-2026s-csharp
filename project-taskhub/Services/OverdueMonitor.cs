namespace TaskHub.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class OverdueMonitor : IDisposable
    {
        private readonly TaskManager _manager;
        private readonly CancellationTokenSource _cts;
        private readonly Task _monitorTask;
        private readonly int _intervalSeconds;

        public OverdueMonitor(TaskManager manager, int intervalSeconds = 10)
        {
            _manager = manager;
            _intervalSeconds = intervalSeconds;
            _cts = new CancellationTokenSource();
            _monitorTask = Task.Run(RunLoopAsync);
        }

        private async Task RunLoopAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_intervalSeconds * 1000, _cts.Token);
                    _manager.CheckOverdueTasks();
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Monitor error: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _monitorTask.Wait(1000); } catch { }
            _cts.Dispose();
        }
    }
}