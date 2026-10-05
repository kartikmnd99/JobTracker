using JobTracker.Application.DTOs;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces;

public interface IJobApplicationRepository
{
	Task<(List<JobApplication> Items, int Total)> GetPagedAsync(int userId, ApplicationQuery query);
	Task<JobApplication?> GetByIdAsync(int userId, int id);
	Task AddAsync(JobApplication application);
	Task SaveAsync();
	Task DeleteAsync(JobApplication application);
}
