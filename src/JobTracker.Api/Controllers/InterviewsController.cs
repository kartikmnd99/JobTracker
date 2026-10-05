using System.Security.Claims;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _service;

    public InterviewsController(IInterviewService service) => _service = service;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("api/applications/{applicationId:int}/interviews")]
    public async Task<IActionResult> GetForApplication(int applicationId)
    {
        var result = await _service.GetForApplicationAsync(UserId, applicationId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("api/applications/{applicationId:int}/interviews")]
    public async Task<IActionResult> Create(int applicationId, InterviewRequest request)
    {
        var created = await _service.CreateAsync(UserId, applicationId, request);
        if (created is null) return NotFound();

        return Created($"/api/interviews/{created.Id}", created);
    }

    [HttpPut("api/interviews/{id:int}")]
    public async Task<IActionResult> Update(int id, InterviewRequest request)
    {
        var result = await _service.UpdateAsync(UserId, id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("api/interviews/{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(UserId, id) ? NoContent() : NotFound();
}