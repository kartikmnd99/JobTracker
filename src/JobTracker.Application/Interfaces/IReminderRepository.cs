using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces;

public interface IReminderRepository
{
    // Interviews scheduled between from and to that have not been reminded yet
    Task<List<Interview>> GetDueAsync(DateTime from, DateTime to);
    Task SaveAsync();
}