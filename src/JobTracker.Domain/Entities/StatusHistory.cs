using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

public class StatusHistory
{
    public int Id { get; set; }
    public int JobApplicationId { get; set; }
    public ApplicationStatus OldStatus { get; set; }
    public ApplicationStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public JobApplication JobApplication { get; set; } = null!;
}