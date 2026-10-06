using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

public class Interview
{
    public int Id { get; set; }
    public int JobApplicationId { get; set; }
    public int Round { get; set; }
    public DateTime ScheduledAt { get; set; }
    public InterviewMode Mode { get; set; }
    public string? Result { get; set; }
    public string? Feedback { get; set; }
    public DateTime? ReminderSentAt { get; set; }   // new

    public JobApplication JobApplication { get; set; } = null!;
}