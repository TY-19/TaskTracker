using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IAssignmentService
{
    public Task<IEnumerable<AssignmentGetModel>> GetAllAssignmentsOfTheBoardAsync(int boardId);
    public Task<AssignmentGetModel?> CreateAssignmentAsync(int boardId, AssignmentPostPutModel assignmentModel);
    public Task<AssignmentGetModel?> GetAssignmentAsync(int boardId, int taskId);
    public Task UpdateAssignmentAsync(int boardId, int taskId, AssignmentPostPutModel model);
    public Task DeleteAssignmentAsync(int boardId, int taskId);
}
