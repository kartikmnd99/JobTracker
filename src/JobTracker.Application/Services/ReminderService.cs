using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;

namespace JobTracker.Application.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _repo;
    private readonly IEmailSender _email;

    public ReminderService(IReminderRepository repo, IEmailSender email)
    {
        _repo = repo;
        _email = email;
    }

    public async Task<ReminderResult> SendDueRemindersAsync()
    {
        var now = DateTime.UtcNow;
        var due = await _repo.GetDueAsync(now, now.AddHours(24));

        int sent = 0, failed = 0;

        foreach (var interview in due)
        {
            var app = interview.JobApplication;
            var user = app.User;

            var subject = $"Interview reminder: {app.CompanyName} (Round {interview.Round})";
            var body =
                $"Hi {user.FullName},\n\n" +
                $"You have an interview with {app.CompanyName} for {app.RoleTitle} " +
                $"(Round {interview.Round}, {interview.Mode}) on " +
                $"{interview.ScheduledAt:dd MMM yyyy, HH:mm} UTC.\n\n" +
                "Good luck!\nJobTracker";

            try
            {
                await _email.SendAsync(user.Email, subject, body);
                interview.ReminderSentAt = now;   // so we never remind twice
                sent++;
            }
            catch
            {
                failed++;   // ReminderSentAt stays null, so the next run retries
            }
        }

        if (sent > 0)
            await _repo.SaveAsync();

        return new ReminderResult(sent, failed);
    }
}