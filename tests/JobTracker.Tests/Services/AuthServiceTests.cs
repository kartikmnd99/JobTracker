using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using JobTracker.Domain.Entities;
using Moq;

namespace JobTracker.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _jwt.Setup(j => j.Generate(It.IsAny<User>()))
            .Returns(("fake-token", DateTime.UtcNow.AddHours(1)));

        _sut = new AuthService(_users.Object, _hasher.Object, _jwt.Object);
    }

    [Fact]
    public async Task Register_NewEmail_HashesPasswordAndReturnsToken()
    {
        _users.Setup(u => u.EmailExistsAsync("kartik@example.com")).ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("Test@12345")).Returns("hashed-value");

        var result = await _sut.RegisterAsync(new RegisterRequest
        {
            FullName = "Kartik Kumar",
            Email = "  Kartik@Example.com ",
            Password = "Test@12345"
        });

        Assert.NotNull(result);
        Assert.Equal("kartik@example.com", result!.Email);   // trimmed and lower-cased
        Assert.Equal("fake-token", result.Token);
        _users.Verify(u => u.AddAsync(It.Is<User>(x => x.PasswordHash == "hashed-value")), Times.Once);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsNullAndDoesNotSave()
    {
        _users.Setup(u => u.EmailExistsAsync("kartik@example.com")).ReturnsAsync(true);

        var result = await _sut.RegisterAsync(new RegisterRequest
        {
            FullName = "Kartik",
            Email = "kartik@example.com",
            Password = "Test@12345"
        });

        Assert.Null(result);
        _users.Verify(u => u.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsNull()
    {
        _users.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "nobody@example.com", Password = "x" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var user = new User { Id = 1, Email = "kartik@example.com", PasswordHash = "hash" };
        _users.Setup(u => u.GetByEmailAsync("kartik@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrong", "hash")).Returns(false);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "kartik@example.com", Password = "wrong" });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_CorrectPassword_ReturnsToken()
    {
        var user = new User { Id = 1, FullName = "Kartik", Email = "kartik@example.com", PasswordHash = "hash" };
        _users.Setup(u => u.GetByEmailAsync("kartik@example.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Test@12345", "hash")).Returns(true);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "kartik@example.com", Password = "Test@12345" });

        Assert.NotNull(result);
        Assert.Equal(1, result!.UserId);
        Assert.Equal("fake-token", result.Token);
    }
}