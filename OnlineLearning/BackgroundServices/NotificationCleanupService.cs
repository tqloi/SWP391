
using OnlineLearningApp.Respositories;
namespace OnlineLearning.BackgroundServices
{
    public class NotificationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationCleanupService> _logger;

        public NotificationCleanupService(IServiceProvider serviceProvider, ILogger<NotificationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Thực hiện xóa thông báo cũ mỗi ngày
                    await CleanupOldNotificationsAsync();

                    // Đợi 24 giờ trước khi thực hiện lại
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up old notifications.");
                }
            }
        }

        private async Task CleanupOldNotificationsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Lấy các thông báo đã quá 7 ngày
                var oldNotifications = dataContext.Notification
                    .Where(n => n.CreatedAt < DateTime.Now.AddDays(-5));

                if (oldNotifications.Any())
                {
                    dataContext.Notification.RemoveRange(oldNotifications);
                    await dataContext.SaveChangesAsync();

                    _logger.LogInformation($"{oldNotifications.Count()} notifications deleted successfully.");
                }
            }
        }
    }

}
