using System.Security.Claims;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _auth;

	public AuthController(IAuthService auth) => _auth = auth;

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterRequest request)
	{
		var result = await _auth.RegisterAsync(request);
		if (result is null)
			return Conflict(new { message = "Email is already registered." });

		return StatusCode(StatusCodes.Status201Created, result);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginRequest request)
	{
		var result = await _auth.LoginAsync(request);
		if (result is null)
			return Unauthorized(new { message = "Invalid email or password." });

		return Ok(result);
	}

	// Used to test that the token works
	[Authorize]
	[HttpGet("me")]
	public IActionResult Me()
	{
		return Ok(new
		{
			userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
			email = User.FindFirstValue(ClaimTypes.Email),
			name = User.FindFirstValue(ClaimTypes.Name)
		});
	}
}