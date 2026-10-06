using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs;

public class DashboardSummary
{
    public int TotalApplications { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public List<WeekCount> ApplicationsPerWeek { get; set; } = new();
    public List<UpcomingInterview> UpcomingInterviews { get; set; } = new();
}

public class WeekCount
{
    public DateTime WeekStart { get; set; }
    public int Count { get; set; }
}

public class UpcomingInterview
{
    public int InterviewId { get; set; }
    public int ApplicationId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public int Round { get; set; }
    public DateTime ScheduledAt { get; set; }
    public InterviewMode Mode { get; set; }
}