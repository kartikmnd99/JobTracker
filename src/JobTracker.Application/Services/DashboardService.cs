using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.Services;

public class DashboardService : IDashboardService
{
    private const int WeeksToShow = 8;

    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo) => _repo = repo;

    public async Task<DashboardSummary> GetSummaryAsync(int userId)
    {
        var counts = await _repo.GetStatusCountsAsync(userId);

        // Show every status, even those with 0
        var byStatus = Enum.GetValues<ApplicationStatus>()
            .ToDictionary(s => s.ToString(), s => counts.GetValueOrDefault(s, 0));

        // Weeks start on Monday
        var today = DateTime.UtcNow.Date;
        var thisWeekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var firstWeekStart = thisWeekStart.AddDays(-7 * (WeeksToShow - 1));

        var dates = await _repo.GetAppliedDatesSinceAsync(userId, firstWeekStart);

        var perWeek = Enumerable.Range(0, WeeksToShow)
            .Select(i => firstWeekStart.AddDays(7 * i))
            .Select(weekStart => new WeekCount
            {
                WeekStart = weekStart,
                Count = dates.Count(d => d >= weekStart && d < weekStart.AddDays(7))
            })
            .ToList();

        var interviews = await _repo.GetUpcomingInterviewsAsync(userId, DateTime.UtcNow, 5);

        return new DashboardSummary
        {
            TotalApplications = byStatus.Values.Sum(),
            ByStatus = byStatus,
            ApplicationsPerWeek = perWeek,
            UpcomingInterviews = interviews.Select(i => new UpcomingInterview
            {
                InterviewId = i.Id,
                ApplicationId = i.JobApplicationId,
                CompanyName = i.JobApplication.CompanyName,
                RoleTitle = i.JobApplication.RoleTitle,
                Round = i.Round,
                ScheduledAt = i.ScheduledAt,
                Mode = i.Mode
            }).ToList()
        };
    }
}