using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IAssignmentService
{
    public Task<IEnumerable<AssignmentGetModel>> GetAllAssignmentsOfTheBoardAsync(int boardId);
    public Task<AssignmentGetModel> CreateAssignmentAsync(int boardId, AssignmentPostModel assignmentModel);
    public Task<AssignmentGetModel?> GetAssignmentAsync(int boardId, int taskId);
    public Task UpdateAssignmentAsync(int boardId, int taskId, AssignmentPutModel model);
    public Task MoveAssignmentToTheStageAsync(int boardId, int taskId, int stageId, string userName);
    public Task ChangeAssignmentStatus(int boardId, int taskId, bool isCompleted, string userName);
    public Task DeleteAssignmentAsync(int boardId, int taskId);
}
