using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    public EmployeeService(ITrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAllAsync()
    {
        var employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        var mapped = _mapper.Map<List<EmployeeGetBoardModel>>(employees);
        return mapped;
    }

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeFromTheBoardAsync(int boardId)
    {
        var employees = await _context.Employees
                .Where(s => s.Boards.Select(b => b.Id).Contains(boardId))
                .AsNoTracking()
                .ToListAsync();

        var mapped = _mapper.Map<List<EmployeeGetBoardModel>>(employees);
        return mapped;
    }

    public async Task<EmployeeGetBoardModel?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
        var mapped = _mapper.Map<EmployeeGetBoardModel>(employee);
        return mapped;
    }

    public async Task AddEmployeeToTheBoardAsync(int boardId, string userNameOrId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Id == userNameOrId || u.UserName == userNameOrId);
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
}
