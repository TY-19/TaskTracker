using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;

namespace TaskTracker.WebAPI.Controllers;

[Route("api/boards/{boardId}/tasks")]
[Authorize]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ISubpartService _subpartService;
    public AssignmentsController(IAssignmentService assignmentService,
        ISubpartService subpartService)
    {
        _assignmentService = assignmentService;
        _subpartService = subpartService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AssignmentGetModel>>> GetAllAssignmentsOfTheBoard(int boardId)
    {
        var assignments = await _assignmentService.GetAllAssignmentsOfTheBoardAsync(boardId);
        return Ok(assignments);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateANewAssignment(int boardId,
        AssignmentPostPutModel model)
    {
        AssignmentGetModel? assignment;
        try
        {
            assignment = await _assignmentService.CreateAssignmentAsync(boardId, model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return CreatedAtAction(nameof(CreateANewAssignment), assignment);
    }

    [Route("{taskId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentGetModel>> GetAssignmentById(int boardId, int taskId)
    {
        var assignment = await _assignmentService.GetAssignmentAsync(boardId, taskId);

        if (assignment is null)
            return NotFound();

        return Ok(assignment);
    }

    [Route("{taskId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateAssignmentById(int boardId, int taskId,
        AssignmentPostPutModel model)
    {
        try
        {
            await _assignmentService.UpdateAssignmentAsync(boardId, taskId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{taskId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAssignmentById(int boardId, int taskId)
    {
        await _assignmentService.DeleteAssignmentAsync(boardId, taskId);
        return NoContent();
    }

    [Route("{taskId}/subparts")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetAllSubpartsOfTheAssignment(
        int boardId, int taskId)
    {
        return Ok(await _subpartService.GetAllSubpartOfTheAssignmentAsync(taskId));
    }

    [Route("{taskId}/subparts/{subpartId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetSubpartById(
        int boardId, int taskId, int subpartId)
    {
        var subpart = await _subpartService.GetSubpartByIdAsync(taskId, subpartId);
        if (subpart == null)
            return NotFound();

        return Ok(subpart);
    }

    [Route("{taskId}/subparts")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSubpartToTheAssignment(int boardId, int taskId,
        SubpartPostPutModel model)
    {
        if (model.AssignmentId != taskId)
            return BadRequest("Not created");

        SubpartGetModel? subpart;
        try
        {
            subpart = await _subpartService.AddSubpartToTheAssignmentAsync(model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return CreatedAtAction(nameof(GetSubpartById), subpart);
    }

    [Route("{taskId}/subparts/{subpartId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSubpart(int boardId, int taskId,
        int subpartId, SubpartPostPutModel model)
    {
        try
        {
            await _subpartService.UpdateSubpartAsync(taskId, subpartId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{taskId}/subparts/{subpartId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSubpart(int boardId, int taskId, int subpartId)
    {
        await _subpartService.DeleteSubpartAsync(taskId, subpartId);
        return NoContent();
    }
}
