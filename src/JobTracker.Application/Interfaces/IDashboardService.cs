using JobTracker.Application.DTOs;

namespace JobTracker.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(int userId);
}
