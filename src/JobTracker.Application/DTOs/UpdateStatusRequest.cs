using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs;

public class UpdateStatusRequest
{
    public ApplicationStatus Status { get; set; }
}