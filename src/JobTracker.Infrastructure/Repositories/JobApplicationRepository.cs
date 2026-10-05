using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly AppDbContext _db;

    public JobApplicationRepository(AppDbContext db) => _db = db;

    public async Task<(List<JobApplication> Items, int Total)> GetPagedAsync(int userId, ApplicationQuery q)
    {
        var query = _db.JobApplications
            .AsNoTracking()
            .Where(a => a.UserId == userId);

        if (q.Status.HasValue)
            query = query.Where(a => a.Status == q.Status.Value);

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim();
            query = query.Where(a => a.CompanyName.Contains(term) || a.RoleTitle.Contains(term));
        }

        var total = await query.CountAsync();

        query = q.SortBy?.ToLowerInvariant() switch
        {
            "company" => q.Desc ? query.OrderByDescending(a => a.CompanyName) : query.OrderBy(a => a.CompanyName),
            "applieddate" => q.Desc ? query.OrderByDescending(a => a.AppliedDate) : query.OrderBy(a => a.AppliedDate),
            "status" => q.Desc ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            _ => q.Desc ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt)
        };

        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public Task<JobApplication?> GetByIdAsync(int userId, int id) =>
        _db.JobApplications
           .Include(a => a.StatusHistories)
           .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

    public async Task AddAsync(JobApplication application)
    {
        _db.JobApplications.Add(application);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();

    public async Task DeleteAsync(JobApplication application)
    {
        _db.JobApplications.Remove(application);
        await _db.SaveChangesAsync();
    }
}