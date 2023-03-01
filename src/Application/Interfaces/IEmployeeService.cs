using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeGetBoardModel>> GetAll();
    public Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeFromTheBoardAsync(int boardId);
    public Task<EmployeeGetBoardModel> GetEmployeeById(int id);
    public Task AddEmployeeToTheBoardAsync(int boardId, UserProfileModel user);
    public Task RemoveEmployeeFromTheBoard(int boardId, int employeeId);
}
