using ApiVideo.Client;
using Firebase.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineLearning.Areas.Instructor.Models;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.BackgroundServices
{
    public class EndLiveStreamService : BackgroundService
    {
        private readonly ApiVideoClient _apiVideoClient;
        private readonly IServiceProvider _serviceProvider;
        public EndLiveStreamService(IOptions<ApiVideoSettings> apiVideoSettings, IServiceProvider serviceProvider)
        {
            _apiVideoClient = new ApiVideoClient(apiVideoSettings.Value.ApiKey);
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await EndScheduledStreamsAsync();
                // Check every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task EndScheduledStreamsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserModel>>();

                // Retrieve all records from LivestreamRecord that have already started
                var allRecords = await dataContext.LivestreamRecord
                    .Where(r => r.ScheduleStartTime <= DateTime.UtcNow)
                    .ToListAsync();

                // Filter records to end based on client-side evaluation
                var streamsToEnd = allRecords
                    .Where(r => r.ScheduleStartTime + r.ScheduleLiveDuration <= DateTime.UtcNow + TimeSpan.FromMinutes(5))
                    .ToList();

                foreach (var record in streamsToEnd)
                {
                    try
                    {
                        var instructor = await userManager.FindByIdAsync(record.UserID);
                        // End the stream in ApiVideo
                        await _apiVideoClient.LiveStreams().completeAsync(record.LivestreamId);
                        //To all student who enrolled in that course
                        var StudentCourse = dataContext.StudentCourses
                            .ToList()
                            .Where(c => c.CourseID == record.CourseID);

                        if (StudentCourse.Count() > 0)
                        {
                            foreach (var student in StudentCourse)
                            {
                                //send a notification to all student announce that a live has ended
                                var notification = new NotificationModel
                                {
                                    CreatedAt = DateTime.UtcNow,
                                    Description = $"[Livestream] {instructor.UserName} has ended a livestream",
                                    UserId = student.StudentID,
                                };
                                dataContext.Add(notification);
                            }
                        }
                        dataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        Console.WriteLine($"Error ending stream {record.LivestreamId}: {ex.Message}");
                    }
                }
            }
        }
    }
}
