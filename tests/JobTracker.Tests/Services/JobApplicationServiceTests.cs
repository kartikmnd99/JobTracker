using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Moq;

namespace JobTracker.Tests;

public class JobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _repo = new();
    private readonly JobApplicationService _sut;

    public JobApplicationServiceTests() => _sut = new JobApplicationService(_repo.Object);

    [Fact]
    public async Task Create_SetsUserIdAndStartsInWishlist()
    {
        var result = await _sut.CreateAsync(7, new JobApplicationRequest
        {
            CompanyName = "  Infosys ",
            RoleTitle = "Junior .NET Developer"
        });

        Assert.Equal("Infosys", result.CompanyName);   // trimmed
        Assert.Equal(ApplicationStatus.Wishlist, result.Status);
        _repo.Verify(r => r.AddAsync(It.Is<JobApplication>(a =>
            a.UserId == 7 && a.Status == ApplicationStatus.Wishlist)), Times.Once);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((JobApplication?)null);

        var result = await _sut.GetByIdAsync(1, 99);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNullAndDoesNotSave()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((JobApplication?)null);

        var result = await _sut.UpdateAsync(1, 99, new JobApplicationRequest
        {
            CompanyName = "X",
            RoleTitle = "Y"
        });

        Assert.Null(result);
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_NotFound_ReturnsNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((JobApplication?)null);

        var result = await _sut.ChangeStatusAsync(1, 99, new UpdateStatusRequest { Status = ApplicationStatus.Applied });

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ChangeStatus_WishlistToApplied_SavesHistoryAndSetsAppliedDate()
    {
        var app = new JobApplication { Id = 1, UserId = 1, Status = ApplicationStatus.Wishlist };
        _repo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(app);

        var result = await _sut.ChangeStatusAsync(1, 1, new UpdateStatusRequest { Status = ApplicationStatus.Applied });

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(ApplicationStatus.Applied, app.Status);
        Assert.NotNull(app.AppliedDate);
        var history = Assert.Single(app.StatusHistories);
        Assert.Equal(ApplicationStatus.Wishlist, history.OldStatus);
        Assert.Equal(ApplicationStatus.Applied, history.NewStatus);
        _repo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Theory]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Wishlist)]
    [InlineData(ApplicationStatus.Wishlist, ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.Interview, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Interview)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Applied)]
    public async Task ChangeStatus_InvalidTransition_ReturnsInvalidAndDoesNotSave(
        ApplicationStatus from, ApplicationStatus to)
    {
        var app = new JobApplication { Id = 1, UserId = 1, Status = from };
        _repo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(app);

        var result = await _sut.ChangeStatusAsync(1, 1, new UpdateStatusRequest { Status = to });

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal(from, app.Status);   // unchanged
        _repo.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_SameStatus_ReturnsInvalid()
    {
        var app = new JobApplication { Id = 1, UserId = 1, Status = ApplicationStatus.Applied };
        _repo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(app);

        var result = await _sut.ChangeStatusAsync(1, 1, new UpdateStatusRequest { Status = ApplicationStatus.Applied });

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task GetAll_ClampsPageAndPageSize()
    {
        _repo.Setup(r => r.GetPagedAsync(1, It.IsAny<ApplicationQuery>()))
             .ReturnsAsync((new List<JobApplication>(), 0));

        var result = await _sut.GetAllAsync(1, new ApplicationQuery { Page = 0, PageSize = 500 });

        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task Delete_Found_DeletesAndReturnsTrue()
    {
        var app = new JobApplication { Id = 1, UserId = 1 };
        _repo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(app);

        var deleted = await _sut.DeleteAsync(1, 1);

        Assert.True(deleted);
        _repo.Verify(r => r.DeleteAsync(app), Times.Once);
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsFalse()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((JobApplication?)null);

        var deleted = await _sut.DeleteAsync(1, 99);

        Assert.False(deleted);
        _repo.Verify(r => r.DeleteAsync(It.IsAny<JobApplication>()), Times.Never);
    }
}