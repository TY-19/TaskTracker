using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IEmployeeService
{
    public IEnumerable<Employee> GetAll();
    public Task<Employee?> GetByIdAsync(int id);
    public Task AddAsync(Employee item);
    public Task UpdateAsync(Employee item);
    public Task DeleteAsync(int id);
}
