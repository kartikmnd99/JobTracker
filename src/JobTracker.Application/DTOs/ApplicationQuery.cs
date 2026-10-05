using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs;

public class ApplicationQuery
{
    public ApplicationStatus? Status { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = "createdAt";   // createdAt | company | appliedDate | status
    public bool Desc { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}