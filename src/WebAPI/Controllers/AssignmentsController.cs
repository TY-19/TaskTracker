using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.Controllers;

[Route("api/boards/{boardId}/tasks")]
[Authorize]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ISubpartService _subpartService;
    private readonly IValidationService _validationService;
    public AssignmentsController(IAssignmentService assignmentService,
        ISubpartService subpartService,
        IValidationService validationService)
    {
        _assignmentService = assignmentService;
        _subpartService = subpartService;
        _validationService = validationService;
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AssignmentGetModel>>> GetAllAssignmentsOfTheBoard(int boardId)
    {
        return Ok(await _assignmentService.GetAllAssignmentsOfTheBoardAsync(boardId));
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateNewAssignment(int boardId,
        AssignmentPostModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        AssignmentGetModel? assignment;
        try
        {
            assignment = await _assignmentService.CreateAssignmentAsync(boardId, model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return CreatedAtAction(nameof(GetAssignmentById),
            new { boardId = assignment.BoardId, taskId = assignment.Id },
            assignment);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentGetModel>> GetAssignmentById(int boardId, int taskId)
    {
        AssignmentGetModel? assignment = await _assignmentService.GetAssignmentAsync(boardId, taskId);

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
        AssignmentPutModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

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

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/move/{stageId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MoveAssignmentToTheStage(int boardId, int taskId, int stageId)
    {
        string? userName = User.Identity?.Name;
        if (userName == null)
            return Unauthorized();
        try
        {
            await _assignmentService.MoveAssignmentToTheStageAsync(boardId, taskId, stageId, userName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/complete")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CompleteAssignmentById(int boardId, int taskId)
    {
        return await ChangeAssignmentStatusAsync(boardId, taskId, true);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/uncomplete")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UncompleteAssignmentById(int boardId, int taskId)
    {
        return await ChangeAssignmentStatusAsync(boardId, taskId, false);
    }

    private async Task<IActionResult> ChangeAssignmentStatusAsync(int boardId, int taskId, bool isCompleted)
    {
        string? userName = User.Identity?.Name;
        if (userName == null)
            return Unauthorized();
        try
        {
            await _assignmentService.ChangeAssignmentStatus(boardId, taskId, isCompleted, userName);
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

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/subparts")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetAllSubpartsOfTheAssignment(int taskId)
    {
        return Ok(await _subpartService.GetAllSubpartOfTheAssignmentAsync(taskId));
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/subparts/{subpartId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetSubpartById(
       int taskId, int subpartId)
    {
        SubpartGetModel? subpart = await _subpartService.GetSubpartByIdAsync(taskId, subpartId);
        if (subpart == null)
            return NotFound();

        return Ok(subpart);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/subparts")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddSubpartToTheAssignment(int boardId, int taskId,
        SubpartPostModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        if (model.AssignmentId != taskId)
            return BadRequest("Not created");

        SubpartGetModel subpart;
        try
        {
            subpart = await _subpartService.AddSubpartToTheAssignmentAsync(model)
                ?? throw new ArgumentException("Subpart wasn't added");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return CreatedAtAction(nameof(GetSubpartById),
            new { boardId, taskId = subpart.AssignmentId, subpartId = subpart.Id },
            subpart);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/subparts/{subpartId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSubpart(int taskId,
        int subpartId, SubpartPutModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

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

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{taskId}/subparts/{subpartId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSubpart(int taskId, int subpartId)
    {
        await _subpartService.DeleteSubpartAsync(taskId, subpartId);
        return NoContent();
    }
}
