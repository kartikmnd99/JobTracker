using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces;

public interface IInterviewRepository
{
    Task<bool> ApplicationExistsAsync(int userId, int applicationId);
    Task<List<Interview>> GetByApplicationAsync(int userId, int applicationId);
    Task<Interview?> GetByIdAsync(int userId, int id);
    Task AddAsync(Interview interview);
    Task SaveAsync();
    Task DeleteAsync(Interview interview);
}