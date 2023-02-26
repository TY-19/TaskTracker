using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ITrackerDbContext _context;
    public EmployeeService(ITrackerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Employee item)
    {
        await _context.Employees.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }

    public IEnumerable<Employee> GetAll()
    {
        return _context.Employees
                .Include(e => e.Assignments)
                .Include(e => e.Boards)
                .AsNoTracking();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
                .Include(e => e.Assignments)
                .Include(e => e.Boards)
                .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task UpdateAsync(Employee item)
    {
        _context.Employees.Update(item);
        await _context.SaveChangesAsync();
    }
}
