using System;
using System.Net.Http;
using BusinessLogic.Execution.Jobs;
using DataAccess.DaRunner;
using FluentScheduler;

namespace BusinessLogic.Execution
{
    public class SchedulerService : IDisposable
    {
        private readonly DaRunner _daRunner;
        private readonly HttpClient _httpClient;

        public SchedulerService(DaRunner daRunner, HttpClient httpClient)
        {
            _daRunner = daRunner;
            _httpClient = httpClient;
            Start();
        }

        private void Start()
        {
            var registry = new Registry();
            registry.NonReentrantAsDefault();
            Setup(registry);
            JobManager.Initialize(registry);
        }

        private void Setup(Registry registry)
        {
            registry.Schedule(() => new PendingBalance(_daRunner, _httpClient))
                .WithName(nameof(PendingBalance))
                .ToRunEvery(15).Seconds();
        }

        public void Dispose()
        {
            JobManager.StopAndBlock();
            _httpClient.Dispose();
        }
    }
}