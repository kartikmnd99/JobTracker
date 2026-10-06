using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Moq;

namespace JobTracker.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummary_FillsMissingStatusesAndWeeks()
    {
        var repo = new Mock<IDashboardRepository>();
        repo.Setup(r => r.GetStatusCountsAsync(1)).ReturnsAsync(new Dictionary<ApplicationStatus, int>
        {
            [ApplicationStatus.Wishlist] = 1,
            [ApplicationStatus.Applied] = 2
        });
        repo.Setup(r => r.GetAppliedDatesSinceAsync(1, It.IsAny<DateTime>())).ReturnsAsync(new List<DateTime>());
        repo.Setup(r => r.GetUpcomingInterviewsAsync(1, It.IsAny<DateTime>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Interview>());

        var result = await new DashboardService(repo.Object).GetSummaryAsync(1);

        Assert.Equal(3, result.TotalApplications);
        Assert.Equal(5, result.ByStatus.Count);          // every status is present
        Assert.Equal(0, result.ByStatus["Rejected"]);    // missing ones are 0
        Assert.Equal(8, result.ApplicationsPerWeek.Count);
        Assert.Empty(result.UpcomingInterviews);
    }
}