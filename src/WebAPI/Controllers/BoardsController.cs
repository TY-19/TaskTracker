using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using AutoMapper;

namespace TaskTracker.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IUserService _userService;
    public BoardsController(IBoardService boardService, IUserService userService)
    {
        _boardService = boardService;
        _userService = userService;
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<IEnumerable<BoardGetModel>> GetAllBoards()
    {
        var boards = _boardService.GetAllBoards();
        return Ok(boards);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateNewBoard(BoardPostPutModel model)
    {
        BoardGetModel? board;
        try
        {
            board = await _boardService.AddBoardAsync(model.Name);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        if (board == null)
            return BadRequest("Not created");

        return CreatedAtAction(nameof(CreateNewBoard), board);
    }

    [Route("{id}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardGetModel>> GetBoard(int id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        if (board == null)
            return NotFound();
        
        return Ok(board);
    }

    [Route("{id}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateBoardName(int id,
        BoardPostPutModel model)
    {
        await _boardService.UpdateBoardNameAsync(id, model.Name);
        return NoContent();
    }

    [Route("{id}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteBoard(int id)
    {
        await _boardService.DeleteBoardAsync(id);
        return NoContent();
    }

    [Route("{boardId}/tasks")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AssignmentGetModel>>> GetAllAssignmentsOfTheBoard(int boardId)
    {
        var assignments = await _boardService.GetAllAssignmentsOfTheBoard(boardId);
        return Ok(assignments);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [Route("{boardId}/tasks")]
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
            assignment = await _boardService.CreateAssignmentAsync(boardId, model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        if (assignment == null)
            return BadRequest("Not created");

        return CreatedAtAction(nameof(CreateANewAssignment), assignment);
    }

    [Route("{boardId}/tasks/{taskId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignmentGetModel>> GetAssignmentById(int boardId, int taskId)
    {
        var assignment = await _boardService.GetAssignmentAsync(boardId, taskId);

        if (assignment is null)
            return NotFound();

        return Ok(assignment);
    }

    [Route("{boardId}/tasks/{taskId}")]
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
            await _boardService.UpdateAssignmentAsync(boardId, taskId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{boardId}/tasks/{taskId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAssignmentById(int boardId, int taskId)
    {
        await _boardService.DeleteAssignmentAsync(boardId, taskId);
        return NoContent();
    }

    [Route("{boardId}/stages")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStagesOfTheBoard(int boardId)
    {
        var stages = await _boardService.GetAllStagesOfTheBoardAsync(boardId);
        return Ok(stages);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [Route("{boardId}/stages")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WorkflowStageGetModel>> CreateANewStageOnTheBoard(
        int boardId, WorkflowStagePostPutModel model)
    {
        var result = await _boardService.AddStageToTheBoardAsync(boardId, model);
        return CreatedAtAction(nameof(GetStageById), result);
    }

    [Route("{boardId}/stages/{stageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStageById(int boardId, int stageId)
    {
        var result = await _boardService.GetStageByIdAsync(boardId, stageId);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Route("{boardId}/stages/{stageId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStageById(int boardId, int stageId, 
        WorkflowStagePostPutModel model)
    {
        try
        {
            await _boardService.UpdateStageAsync(boardId, stageId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{boardId}/stages/{stageId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteStageById(int boardId, int stageId)
    {
        await _boardService.DeleteStageAsync(boardId, stageId);
        return NoContent();
    }

    [Route("{boardId}/employees")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEmployeesOfTheBoard(int boardId)
    {
        return Ok(await _boardService.GetAllEmployeeAsync(boardId));
    }

    [Route("{boardId}/employees/{employeeId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeById(int boardId, int employeeId)
    {
        var employee = await _boardService.GetEmployeeById(employeeId);
        if (employee == null)
            return NotFound();
        
        return Ok(employee);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [Route("{boardId}/boards/{userNameOrId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddEmployeeToTheBoard(
        int boardId, string userNameOrId)
    {
        var user = await _userService.GetUserByNameOrIdAsync(userNameOrId);
        if (user == null)
            return BadRequest();
        try
        {
            await _boardService.AddEmployeeToTheBoardAsync(boardId, user);
        }
        catch
        {
            return BadRequest();
        }
        return NoContent();
    }

    [Route("{boardId}/employees/{employeeId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveEmployeeFromTheBoard(int boardId, int employeeId)
    {
        await _boardService.RemoveEmployeeFromTheBoard(boardId, employeeId);
        return NoContent();
    }
}
