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

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAll()
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

    public async Task<EmployeeGetBoardModel> GetEmployeeById(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
        var mapped = _mapper.Map<EmployeeGetBoardModel>(employee);
        return mapped;
    }

    public async Task AddEmployeeToTheBoardAsync(int boardId, UserProfileModel user)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e =>
            e.User != null && e.User.Id == user.Id);
        if (employee == null)
        {
            employee = new Employee
            {
                User = new User { Id = user.Id }
            };
            if (user.EmployeeId != null)
                employee.Id = (int)user.EmployeeId;
            if (user.FirstName != null)
                employee.FirstName = user.FirstName;
            if (user.LastName != null)
                employee.LastName = user.LastName;
            employee.Boards = new List<Board>();
        }
        var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null)
            throw new ArgumentException("Board with a such id does not exist");

        employee.Boards.Add(board);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmployeeFromTheBoard(int boardId, int employeeId)
    {
        var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);
        var board = await _context.Boards
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (employee == null || board == null)
            return;

        board.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
