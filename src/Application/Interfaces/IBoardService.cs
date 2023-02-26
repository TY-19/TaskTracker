using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IBoardService
{
    public IEnumerable<BoardGetModel> GetAllBoards();
    public Task<BoardGetModel?> GetBoardByIdAsync(int id);
    public Task<BoardGetModel?> GetBoardByNameAsync(string name);
    public Task<BoardGetModel?> AddBoardAsync(string name);
    public Task UpdateBoardNameAsync(int id, string newName);
    public Task DeleteBoardAsync(int id);

    public Task<IEnumerable<AssignmentGetModel>> GetAllAssignmentsOfTheBoard(int boardId);
    public Task<AssignmentGetModel?> CreateAssignmentAsync(int boardId, AssignmentPostPutModel assignmentModel);
    public Task<AssignmentGetModel?> GetAssignmentAsync(int boardId, int taskId);
    public Task UpdateAssignmentAsync(int boardId, int taskId, AssignmentPostPutModel model);
    public Task DeleteAssignmentAsync(int boardId, int taskId);

    public Task<IEnumerable<WorkflowStageGetModel>> GetAllStagesOfTheBoardAsync(int boardId);
    public Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostPutModel model);
    public Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId);
    public Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePostPutModel model);
    public Task DeleteStageAsync(int boardId, int stageId);

    public Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeAsync(int? boardId);
    public Task<EmployeeGetBoardModel> GetEmployeeById(int id);
    public Task AddEmployeeToTheBoardAsync(int boardId, UserProfileModel user);
    public Task RemoveEmployeeFromTheBoard(int boardId, int employeeId);
}
