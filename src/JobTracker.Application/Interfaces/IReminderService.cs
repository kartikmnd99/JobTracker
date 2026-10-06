using JobTracker.Application.DTOs;

namespace JobTracker.Application.Interfaces;

public interface IReminderService
{
    Task<ReminderResult> SendDueRemindersAsync();
}