using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeGetModel>> GetAllAsync();
    public Task<IEnumerable<EmployeeGetModel>> GetAllEmployeeFromTheBoardAsync(int boardId);
    public Task<EmployeeGetModel?> GetEmployeeByIdAsync(int id);
    public Task AddEmployeeToTheBoardAsync(int boardId, string userNameOrId);
    public Task RemoveEmployeeFromTheBoardAsync(int boardId, int employeeId);
}
