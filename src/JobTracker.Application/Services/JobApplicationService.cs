using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IJobApplicationRepository _repo;

    public JobApplicationService(IJobApplicationRepository repo) => _repo = repo;

    // Which status can move to which
    private static readonly Dictionary<ApplicationStatus, ApplicationStatus[]> AllowedTransitions = new()
    {
        [ApplicationStatus.Wishlist] = new[] { ApplicationStatus.Applied, ApplicationStatus.Rejected },
        [ApplicationStatus.Applied] = new[] { ApplicationStatus.Interview, ApplicationStatus.Offer, ApplicationStatus.Rejected },
        [ApplicationStatus.Interview] = new[] { ApplicationStatus.Offer, ApplicationStatus.Rejected },
        [ApplicationStatus.Offer] = new[] { ApplicationStatus.Rejected },
        [ApplicationStatus.Rejected] = Array.Empty<ApplicationStatus>()
    };

    public async Task<PagedResult<JobApplicationResponse>> GetAllAsync(int userId, ApplicationQuery query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, total) = await _repo.GetPagedAsync(userId, query);

        return new PagedResult<JobApplicationResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    public async Task<JobApplicationResponse?> GetByIdAsync(int userId, int id)
    {
        var app = await _repo.GetByIdAsync(userId, id);
        return app is null ? null : ToResponse(app);
    }

    public async Task<JobApplicationResponse> CreateAsync(int userId, JobApplicationRequest request)
    {
        var app = new JobApplication { UserId = userId, Status = ApplicationStatus.Wishlist };
        Apply(app, request);

        await _repo.AddAsync(app);
        return ToResponse(app);
    }

    public async Task<JobApplicationResponse?> UpdateAsync(int userId, int id, JobApplicationRequest request)
    {
        var app = await _repo.GetByIdAsync(userId, id);
        if (app is null) return null;

        Apply(app, request);
        app.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
        return ToResponse(app);
    }

    public async Task<ServiceResult<JobApplicationResponse>> ChangeStatusAsync(
        int userId, int id, UpdateStatusRequest request)
    {
        var app = await _repo.GetByIdAsync(userId, id);
        if (app is null) return ServiceResult<JobApplicationResponse>.NotFound();

        if (app.Status == request.Status)
            return ServiceResult<JobApplicationResponse>.Invalid($"Application is already {app.Status}.");

        if (!AllowedTransitions[app.Status].Contains(request.Status))
            return ServiceResult<JobApplicationResponse>.Invalid(
                $"Cannot change status from {app.Status} to {request.Status}.");

        app.StatusHistories.Add(new StatusHistory
        {
            OldStatus = app.Status,
            NewStatus = request.Status
        });

        app.Status = request.Status;
        app.UpdatedAt = DateTime.UtcNow;

        // Set the applied date automatically the first time it moves to Applied
        if (request.Status == ApplicationStatus.Applied && app.AppliedDate is null)
            app.AppliedDate = DateTime.UtcNow;

        await _repo.SaveAsync();
        return ServiceResult<JobApplicationResponse>.Ok(ToResponse(app));
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var app = await _repo.GetByIdAsync(userId, id);
        if (app is null) return false;

        await _repo.DeleteAsync(app);
        return true;
    }

    private static void Apply(JobApplication app, JobApplicationRequest r)
    {
        app.CompanyName = r.CompanyName.Trim();
        app.RoleTitle = r.RoleTitle.Trim();
        app.JobUrl = r.JobUrl?.Trim();
        app.Location = r.Location?.Trim();
        app.Salary = r.Salary;
        app.AppliedDate = r.AppliedDate;
        app.Notes = r.Notes;
    }

    private static JobApplicationResponse ToResponse(JobApplication a) => new()
    {
        Id = a.Id,
        CompanyName = a.CompanyName,
        RoleTitle = a.RoleTitle,
        JobUrl = a.JobUrl,
        Location = a.Location,
        Salary = a.Salary,
        Status = a.Status,
        AppliedDate = a.AppliedDate,
        Notes = a.Notes,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}