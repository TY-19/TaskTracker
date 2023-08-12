using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.WebAPI.Configuration.AuthorizationHandlers;

namespace TaskTracker.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IValidationService _validationService;
    public BoardsController(IBoardService boardService,
        IValidationService validationService)
    {
        _boardService = boardService;
        _validationService = validationService;
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<BoardGetModel>>> GetAllBoards()
    {
        return Ok(await _boardService.GetAllBoardsAsync());
    }

    [Route("accessible")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<BoardGetModel>>> GetBoardsOfTheEmployee()
    {
        string? userName = User.Identity?.Name;
        if (userName == null)
            return Unauthorized();

        return Ok(await _boardService.GetBoardOfTheEmployeeAsync(userName));
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateNewBoard(BoardPostModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

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

        return CreatedAtAction(nameof(GetBoard), new { boardId = board.Id }, board);
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{boardId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardGetModel>> GetBoard(int boardId)
    {
        BoardGetModel? board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound();

        return Ok(board);
    }

    [Route("{boardId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateBoardName(int boardId, BoardPutModel model)
    {
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        try
        {
            await _boardService.UpdateBoardNameAsync(boardId, model.Name);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{boardId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteBoard(int boardId)
    {
        await _boardService.DeleteBoardAsync(boardId);
        return NoContent();
    }
}
