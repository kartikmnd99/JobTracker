using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.Interfaces;

public interface IDashboardRepository
{
    Task<Dictionary<ApplicationStatus, int>> GetStatusCountsAsync(int userId);
    Task<List<DateTime>> GetAppliedDatesSinceAsync(int userId, DateTime since);
    Task<List<Interview>> GetUpcomingInterviewsAsync(int userId, DateTime from, int count);
}