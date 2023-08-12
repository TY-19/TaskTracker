using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.Controllers;

[Authorize]
[Route("api/boards/{boardId}/[controller]")]
[ApiController]
public class StagesController : ControllerBase
{
    private readonly IStageService _stageService;
    private readonly IValidationService _validationService;
    public StagesController(IStageService stageService,
        IValidationService validationService)
    {
        _stageService = stageService;
        _validationService = validationService;
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<WorkflowStageGetModel>>> GetAllStagesOfTheBoard(int boardId)
    {
        return Ok(await _stageService.GetAllStagesOfTheBoardAsync(boardId));
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WorkflowStageGetModel>> CreateNewStageOnTheBoard(
        int boardId, WorkflowStagePostModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        WorkflowStageGetModel result;
        try
        {
            result = await _stageService.AddStageToTheBoardAsync(boardId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return CreatedAtAction(nameof(GetStageById),
            new { boardId = result.BoardId, stageId = result.Id },
            result);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{stageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowStageGetModel>> GetStageById(int boardId, int stageId)
    {
        WorkflowStageGetModel? result = await _stageService.GetStageByIdAsync(boardId, stageId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Route("{stageId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStageById(int boardId, int stageId,
        WorkflowStagePutModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        try
        {
            await _stageService.UpdateStageAsync(boardId, stageId, model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{stageId}/moveforward")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MoveStageForward(int boardId, int stageId)
    {
        return await MoveStageAsync(boardId, stageId, true);
    }

    [Route("{stageId}/moveback")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MoveStageBack(int boardId, int stageId)
    {
        return await MoveStageAsync(boardId, stageId, false);
    }

    private async Task<IActionResult> MoveStageAsync(int boardId, int stageId, bool isMovingForward)
    {
        try
        {
            await _stageService.MoveStage(boardId, stageId, isMovingForward);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{stageId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteStageById(int boardId, int stageId)
    {
        try
        {
            await _stageService.DeleteStageAsync(boardId, stageId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }
}
