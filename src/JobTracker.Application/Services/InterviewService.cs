using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repo;

    public InterviewService(IInterviewRepository repo) => _repo = repo;

    // null = application not found (or not yours)
    public async Task<List<InterviewResponse>?> GetForApplicationAsync(int userId, int applicationId)
    {
        if (!await _repo.ApplicationExistsAsync(userId, applicationId))
            return null;

        var items = await _repo.GetByApplicationAsync(userId, applicationId);
        return items.Select(ToResponse).ToList();
    }

    public async Task<InterviewResponse?> CreateAsync(int userId, int applicationId, InterviewRequest request)
    {
        if (!await _repo.ApplicationExistsAsync(userId, applicationId))
            return null;

        var interview = new Interview { JobApplicationId = applicationId };
        Apply(interview, request);

        await _repo.AddAsync(interview);
        return ToResponse(interview);
    }

    public async Task<InterviewResponse?> UpdateAsync(int userId, int id, InterviewRequest request)
    {
        var interview = await _repo.GetByIdAsync(userId, id);
        if (interview is null) return null;

        Apply(interview, request);
        await _repo.SaveAsync();
        return ToResponse(interview);
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var interview = await _repo.GetByIdAsync(userId, id);
        if (interview is null) return false;

        await _repo.DeleteAsync(interview);
        return true;
    }

    private static void Apply(Interview i, InterviewRequest r)
    {
        i.Round = r.Round;
        i.ScheduledAt = r.ScheduledAt.ToUniversalTime();   // store everything in UTC
        i.Mode = r.Mode;
        i.Result = r.Result?.Trim();
        i.Feedback = r.Feedback;
    }

    private static InterviewResponse ToResponse(Interview i) => new()
    {
        Id = i.Id,
        JobApplicationId = i.JobApplicationId,
        Round = i.Round,
        ScheduledAt = i.ScheduledAt,
        Mode = i.Mode,
        Result = i.Result,
        Feedback = i.Feedback
    };
}