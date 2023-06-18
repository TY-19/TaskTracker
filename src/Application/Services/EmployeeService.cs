using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    public EmployeeService(ITrackerDbContext context,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAllAsync()
    {
        var employees = await _context.Employees
                .Include(e => e.User)
                .AsNoTracking()
                .ToListAsync();
        return await GetEmployeeModelsWithRolesAsync(employees);
    }

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeFromTheBoardAsync(int boardId)
    {
        var employees = _context.Employees
                .Include(e => e.User)
                .Where(s => s.Boards.Select(b => b.Id).Contains(boardId))
                .AsNoTracking();
        return await GetEmployeeModelsWithRolesAsync(employees);
    }

    public async Task<EmployeeGetBoardModel?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return null;

        if(employee.User != null)
        {
            return await GetEmployeeModelWithRolesAsync(employee);
        }
        else
        {
            return _mapper.Map<EmployeeGetBoardModel>(employee);
        }
    }

    public async Task AddEmployeeToTheBoardAsync(int boardId, string userNameOrId)
    {
        var user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userNameOrId || u.UserName == userNameOrId);
        if (user == null)
            throw new ArgumentException("User with a such id or name does not exist", nameof(userNameOrId));

        user.Employee ??= new Employee();

        var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);

        if (board == null)
            throw new ArgumentException("Board with a such id does not exist", nameof(boardId));

        board.Employees.Add(user.Employee);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmployeeFromTheBoardAsync(int boardId, int employeeId)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        var board = await _context.Boards
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (employee == null || board == null)
            return;

        board.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }

    private async Task<IEnumerable<EmployeeGetBoardModel>> GetEmployeeModelsWithRolesAsync(IEnumerable<Employee> employees)
    {
        var models = new List<EmployeeGetBoardModel>();
        foreach (var employee in employees)
        {
            models.Add(await GetEmployeeModelWithRolesAsync(employee));
        }
        return models;
    }
    private async Task<EmployeeGetBoardModel> GetEmployeeModelWithRolesAsync(Employee employee)
    {
        IList<string> roles = new List<string>();
        if (employee.User != null)
        {
            roles = await _userManager.GetRolesAsync(employee.User);
        }
        var model = _mapper.Map<EmployeeGetBoardModel>(employee);
        model.Roles = roles;
        return model;
    }
}
