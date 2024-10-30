using Microsoft.AspNetCore.SignalR;

namespace OnlineLearning.Hubs
{
    public class TestHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has join");
        }
        public async Task NotifyTestStart(int testId)
        {
            await Clients.All.SendAsync("TestStarted", testId);
        }
    }
}
