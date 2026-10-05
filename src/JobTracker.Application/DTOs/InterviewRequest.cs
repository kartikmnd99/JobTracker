using System.ComponentModel.DataAnnotations;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs;

public class InterviewRequest
{
    [Range(1, 10)]
    public int Round { get; set; } = 1;

    [Required]
    public DateTime ScheduledAt { get; set; }

    public InterviewMode Mode { get; set; } = InterviewMode.Online;

    [MaxLength(100)]
    public string? Result { get; set; }

    public string? Feedback { get; set; }
}