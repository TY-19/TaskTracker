using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeGetBoardModel>> GetAllAsync();
    public Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeFromTheBoardAsync(int boardId);
    public Task<EmployeeGetBoardModel?> GetEmployeeByIdAsync(int id);
    public Task AddEmployeeToTheBoardAsync(int boardId, string userNameOrId);
    public Task RemoveEmployeeFromTheBoardAsync(int boardId, int employeeId);
}
