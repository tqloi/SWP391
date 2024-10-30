using Microsoft.AspNetCore.SignalR;

namespace OnlineLearning.Hubs
{
    public class TestHub : Hub
    {
        public async Task NotifyTestStart(int testId)
        {
            await Clients.All.SendAsync("TestStarted", testId);
        }
    }
}
