using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;

namespace TaskTracker.WebAPI.Controllers;

[Authorize]
[Route("api/boards/{boardId}/[controller]")]
[ApiController]
public class StagesController : ControllerBase
{
    private readonly IStageService _stageService;
    public StagesController(IStageService stageService)
    {
        _stageService = stageService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<WorkflowStageGetModel>>> GetAllStagesOfTheBoard(int boardId)
    {
        var stages = await _stageService.GetAllStagesOfTheBoardAsync(boardId);
        return Ok(stages);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WorkflowStageGetModel>> CreateANewStageOnTheBoard(
        int boardId, WorkflowStagePostModel model)
    {
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

    [Route("{stageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowStageGetModel>> GetStageById(int boardId, int stageId)
    {
        var result = await _stageService.GetStageByIdAsync(boardId, stageId);
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
        try
        {
            await _stageService.UpdateStageAsync(boardId, stageId, model);
        }
        catch (Exception ex)
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
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.Message}");
        }
    }
}
