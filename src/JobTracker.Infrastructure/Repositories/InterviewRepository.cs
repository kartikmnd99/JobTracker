using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly AppDbContext _db;

    public InterviewRepository(AppDbContext db) => _db = db;

    public Task<bool> ApplicationExistsAsync(int userId, int applicationId) =>
        _db.JobApplications.AnyAsync(a => a.Id == applicationId && a.UserId == userId);

    public Task<List<Interview>> GetByApplicationAsync(int userId, int applicationId) =>
        _db.Interviews
           .AsNoTracking()
           .Where(i => i.JobApplicationId == applicationId && i.JobApplication.UserId == userId)
           .OrderBy(i => i.Round)
           .ToListAsync();

    public Task<Interview?> GetByIdAsync(int userId, int id) =>
        _db.Interviews
           .FirstOrDefaultAsync(i => i.Id == id && i.JobApplication.UserId == userId);

    public async Task AddAsync(Interview interview)
    {
        _db.Interviews.Add(interview);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();

    public async Task DeleteAsync(Interview interview)
    {
        _db.Interviews.Remove(interview);
        await _db.SaveChangesAsync();
    }
}