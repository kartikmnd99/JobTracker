using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs;

public class JobApplicationResponse
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string? JobUrl { get; set; }
    public string? Location { get; set; }
    public decimal? Salary { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime? AppliedDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}