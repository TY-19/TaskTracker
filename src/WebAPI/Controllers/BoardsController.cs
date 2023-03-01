using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;

namespace TaskTracker.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IUserService _userService;
    private readonly IStageService _stageService;
    private readonly IAssignmentService _assignmentService;
    private readonly IEmployeeService _employeeService;
    private readonly ISubpartService _subpartService;
    public BoardsController(IBoardService boardService, IUserService userService,
        IStageService stageService, IAssignmentService assignmentService,
        IEmployeeService employeeService, ISubpartService subpartService)
    {
        _boardService = boardService;
        _userService = userService;
        _stageService = stageService;
        _assignmentService = assignmentService;
        _employeeService = employeeService;
        _subpartService = subpartService;
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
        var assignments = await _assignmentService.GetAllAssignmentsOfTheBoard(boardId);
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
            assignment = await _assignmentService.CreateAssignmentAsync(boardId, model);
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
        var assignment = await _assignmentService.GetAssignmentAsync(boardId, taskId);

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
            await _assignmentService.UpdateAssignmentAsync(boardId, taskId, model);
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
        await _assignmentService.DeleteAssignmentAsync(boardId, taskId);
        return NoContent();
    }

    [Route("{boardId}/tasks/{taskId}/subparts")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetAllSubpartsOfTheAssignment(
        int boardId, int taskId)
    {
        return Ok(await _subpartService.GetAllSubpartOfTheAssignmentAsync(taskId));
    }

    [Route("{boardId}/tasks/{taskId}/subparts/{subpartId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SubpartGetModel>>> GetSubpartById(
        int boardId, int taskId, int subpartId)
    {
        var subpart = await _subpartService.GetSubpartByIdAsync(subpartId);
        if (subpart == null)
            return NotFound();

        return Ok(subpart);
    }

    [Route("{boardId}/tasks/{taskId}/subparts")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSubpartToTheAssignment(int boardId, int taskId,
        SubpartPostPutModel model)
    {
        SubpartGetModel? subpart;
        try
        {
            subpart = await _subpartService.AddSubpartToTheAssignmentAsync(model);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        if (subpart == null)
            return BadRequest("Not created");

        return CreatedAtAction(nameof(GetSubpartById), subpart);
    }

    [Route("{boardId}/tasks/{taskId}/subparts/{subpartId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSubpart(int boardId, int taskId,
        int subpartId, SubpartPostPutModel model)
    {
        try
        {
            await _subpartService.UpdateSubpartAsync(subpartId, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{boardId}/tasks/{taskId}/subparts/{subpartId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSubpart(int boardId, int taskId, int subpartId)
    {
        await _subpartService.DeleteSubpartAsync(subpartId);
        return NoContent();
    }

    [Route("{boardId}/stages")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStagesOfTheBoard(int boardId)
    {
        var stages = await _stageService.GetAllStagesOfTheBoardAsync(boardId);
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
        var result = await _stageService.AddStageToTheBoardAsync(boardId, model);
        return CreatedAtAction(nameof(GetStageById), result);
    }

    [Route("{boardId}/stages/{stageId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStageById(int boardId, int stageId)
    {
        var result = await _stageService.GetStageByIdAsync(boardId, stageId);
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
            await _stageService.UpdateStageAsync(boardId, stageId, model);
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
        await _stageService.DeleteStageAsync(boardId, stageId);
        return NoContent();
    }

    [Route("{boardId}/employees")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEmployeesOfTheBoard(int boardId)
    {
        return Ok(await _employeeService.GetAllEmployeeFromTheBoardAsync(boardId));
    }

    [Route("{boardId}/employees/{employeeId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeById(int boardId, int employeeId)
    {
        var employee = await _employeeService.GetEmployeeById(employeeId);
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
            await _employeeService.AddEmployeeToTheBoardAsync(boardId, user);
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
        await _employeeService.RemoveEmployeeFromTheBoard(boardId, employeeId);
        return NoContent();
    }
}
