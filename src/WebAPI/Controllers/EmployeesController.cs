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
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [Route("~/api/employees")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<EmployeeGetModel>>> GetAllEmployees()
    {
        return Ok(await _employeeService.GetAllAsync());
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<EmployeeGetModel>>> GetAllEmployeesOfTheBoard(int boardId)
    {
        return Ok(await _employeeService.GetAllEmployeeFromTheBoardAsync(boardId));
    }

    [Authorize(AuthorizationPoliciesNames.RESPONSIBLE_EMPLOYEE_POLICY)]
    [Route("{employeeId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeGetModel>> GetEmployeeById(int employeeId)
    {
        EmployeeGetModel? employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [Route("{userNameOrId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddEmployeeToTheBoard(
        int boardId, string userNameOrId)
    {
        try
        {
            await _employeeService.AddEmployeeToTheBoardAsync(boardId, userNameOrId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }

    [Route("{employeeId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveEmployeeFromTheBoard(int boardId, int employeeId)
    {
        await _employeeService.RemoveEmployeeFromTheBoardAsync(boardId, employeeId);
        return NoContent();
    }
}
