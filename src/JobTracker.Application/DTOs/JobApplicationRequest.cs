using System.ComponentModel.DataAnnotations;

namespace JobTracker.Application.DTOs;

public class JobApplicationRequest
{
    [Required, MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string RoleTitle { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? JobUrl { get; set; }

    [MaxLength(150)]
    public string? Location { get; set; }

    [Range(0, 1000000000)]
    public decimal? Salary { get; set; }

    public DateTime? AppliedDate { get; set; }
    public string? Notes { get; set; }
}