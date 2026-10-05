using JobTracker.Application.DTOs;

namespace JobTracker.Application.Interfaces;

public interface IJobApplicationService
{
    Task<PagedResult<JobApplicationResponse>> GetAllAsync(int userId, ApplicationQuery query);
    Task<JobApplicationResponse?> GetByIdAsync(int userId, int id);
    Task<JobApplicationResponse> CreateAsync(int userId, JobApplicationRequest request);
    Task<JobApplicationResponse?> UpdateAsync(int userId, int id, JobApplicationRequest request);
    Task<ServiceResult<JobApplicationResponse>> ChangeStatusAsync(int userId, int id, UpdateStatusRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}