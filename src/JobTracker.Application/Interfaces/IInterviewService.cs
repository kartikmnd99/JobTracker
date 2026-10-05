using JobTracker.Application.DTOs;

namespace JobTracker.Application.Interfaces;

public interface IInterviewService
{
    Task<List<InterviewResponse>?> GetForApplicationAsync(int userId, int applicationId);
    Task<InterviewResponse?> CreateAsync(int userId, int applicationId, InterviewRequest request);
    Task<InterviewResponse?> UpdateAsync(int userId, int id, InterviewRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}