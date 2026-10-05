using System.Security.Claims;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
	private readonly IJobApplicationService _service;

	public ApplicationsController(IJobApplicationService service) => _service = service;

	private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] ApplicationQuery query) =>
		Ok(await _service.GetAllAsync(UserId, query));

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var result = await _service.GetByIdAsync(UserId, id);
		return result is null ? NotFound() : Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(JobApplicationRequest request)
	{
		var created = await _service.CreateAsync(UserId, request);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, JobApplicationRequest request)
	{
		var result = await _service.UpdateAsync(UserId, id, request);
		return result is null ? NotFound() : Ok(result);
	}

	[HttpPatch("{id:int}/status")]
	public async Task<IActionResult> ChangeStatus(int id, UpdateStatusRequest request)
	{
		var result = await _service.ChangeStatusAsync(UserId, id, request);

		return result.Status switch
		{
			ResultStatus.Ok => Ok(result.Data),
			ResultStatus.NotFound => NotFound(),
			_ => BadRequest(new { message = result.Error })
		};
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id) =>
		await _service.DeleteAsync(UserId, id) ? NoContent() : NotFound();
}