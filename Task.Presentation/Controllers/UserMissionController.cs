using Microsoft.AspNetCore.Mvc;
using Task.Application.DTOs.Requests;
using Task.Application.Interfaces.Services;

namespace Task.Presentation.Controllers;

[ApiController]
[Route("api/user-missions")]
public class UserMissionController : ControllerBase
{
    private readonly IUserMissionService _service;

    public UserMissionController(IUserMissionService service)
    {
        _service = service;
    }

    [HttpPost("assign")]
    public async System.Threading.Tasks.Task<IActionResult> AssignMission(AssignMissionRequest request)
    {
        var missionId = await _service.AssignMissionAsync(request);

        return Ok(new
        {
            MissionId = missionId,
            Message = "Mission assigned successfully."
        });
    }

    [HttpGet("user/{userId}")]
    public async System.Threading.Tasks.Task<IActionResult> GetByUser(Guid userId)
    {
        var result = await _service.GetUserMissionsAsync(userId);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetMissionByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}