using Microsoft.AspNetCore.SignalR;
using OnlineLearning.Hubs;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Services
{
    public class TestSchedulerService : IHostedService, IDisposable
    {
        private readonly DataContext _context;
        private readonly ILogger<TestSchedulerService> _logger;
        private readonly IHubContext<TestHub> _hubContext;
        private Timer _timer;

        public TestSchedulerService(DataContext context, ILogger<TestSchedulerService> logger, IHubContext<TestHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set the timer to check for tests every minute
            _timer = new Timer(CheckForStartingTests, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void CheckForStartingTests(object state)
        {
            var now = DateTime.Now;
            var testsToStart = _context.Test
                .Where(t => t.StartTime <= now && t.Status == "Inactive")
                .ToList();

            foreach (var test in testsToStart)
            {
                // Start the test
                await StartTest(test.TestID);

                // Optionally update the test status
                test.Status = "Active";
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();
        }

        private async Task StartTest(int testId)
        {
            await _hubContext.Clients.All.SendAsync("TestStarted", testId);
            _logger.LogInformation($"Test {testId} has started.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}