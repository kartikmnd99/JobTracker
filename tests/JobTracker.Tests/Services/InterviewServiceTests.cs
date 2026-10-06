using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using Moq;

namespace JobTracker.Tests;

public class InterviewServiceTests
{
    private readonly Mock<IInterviewRepository> _repo = new();
    private readonly InterviewService _sut;

    public InterviewServiceTests() => _sut = new InterviewService(_repo.Object);

    [Fact]
    public async Task Create_ApplicationNotOwnedByUser_ReturnsNullAndDoesNotSave()
    {
        _repo.Setup(r => r.ApplicationExistsAsync(1, 5)).ReturnsAsync(false);

        var result = await _sut.CreateAsync(1, 5, new InterviewRequest { ScheduledAt = DateTime.UtcNow.AddDays(1) });

        Assert.Null(result);
        _repo.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task Create_ValidApplication_SavesInterviewForThatApplication()
    {
        _repo.Setup(r => r.ApplicationExistsAsync(1, 5)).ReturnsAsync(true);

        var result = await _sut.CreateAsync(1, 5, new InterviewRequest
        {
            Round = 2,
            ScheduledAt = DateTime.UtcNow.AddDays(3),
            Mode = InterviewMode.Onsite
        });

        Assert.NotNull(result);
        Assert.Equal(5, result!.JobApplicationId);
        Assert.Equal(2, result.Round);
        Assert.Equal(InterviewMode.Onsite, result.Mode);
        _repo.Verify(r => r.AddAsync(It.Is<Interview>(i => i.JobApplicationId == 5)), Times.Once);
    }

    [Fact]
    public async Task GetForApplication_ApplicationNotFound_ReturnsNull()
    {
        _repo.Setup(r => r.ApplicationExistsAsync(1, 99)).ReturnsAsync(false);

        var result = await _sut.GetForApplicationAsync(1, 99);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((Interview?)null);

        var result = await _sut.UpdateAsync(1, 99, new InterviewRequest { ScheduledAt = DateTime.UtcNow });

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsFalse()
    {
        _repo.Setup(r => r.GetByIdAsync(1, 99)).ReturnsAsync((Interview?)null);

        Assert.False(await _sut.DeleteAsync(1, 99));
    }
}