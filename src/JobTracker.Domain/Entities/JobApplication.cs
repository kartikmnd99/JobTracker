using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

public class JobApplication
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string RoleTitle { get; set; } = string.Empty;
	public string? JobUrl { get; set; }
	public string? Location { get; set; }
	public decimal? Salary { get; set; }
	public ApplicationStatus Status { get; set; } = ApplicationStatus.Wishlist;
	public DateTime? AppliedDate { get; set; }
	public string? Notes { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }

	public User User { get; set; } = null!;
	public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
	public ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
}