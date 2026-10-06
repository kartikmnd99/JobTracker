using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _db;

    public ReminderRepository(AppDbContext db) => _db = db;

    public Task<List<Interview>> GetDueAsync(DateTime from, DateTime to) =>
        _db.Interviews
           .Include(i => i.JobApplication)
               .ThenInclude(a => a.User)
           .Where(i => i.ReminderSentAt == null
                    && i.ScheduledAt >= from
                    && i.ScheduledAt <= to)
           .ToListAsync();

    public Task SaveAsync() => _db.SaveChangesAsync();
}