using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _db;

    public DashboardRepository(AppDbContext db) => _db = db;

    public Task<Dictionary<ApplicationStatus, int>> GetStatusCountsAsync(int userId) =>
        _db.JobApplications
           .AsNoTracking()
           .Where(a => a.UserId == userId)
           .GroupBy(a => a.Status)
           .Select(g => new { Status = g.Key, Count = g.Count() })
           .ToDictionaryAsync(x => x.Status, x => x.Count);

    public Task<List<DateTime>> GetAppliedDatesSinceAsync(int userId, DateTime since) =>
        _db.JobApplications
           .AsNoTracking()
           .Where(a => a.UserId == userId && a.AppliedDate != null && a.AppliedDate >= since)
           .Select(a => a.AppliedDate!.Value)
           .ToListAsync();

    public Task<List<Interview>> GetUpcomingInterviewsAsync(int userId, DateTime from, int count) =>
        _db.Interviews
           .AsNoTracking()
           .Include(i => i.JobApplication)
           .Where(i => i.JobApplication.UserId == userId && i.ScheduledAt >= from)
           .OrderBy(i => i.ScheduledAt)
           .Take(count)
           .ToListAsync();
}