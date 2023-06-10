using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IStageService
{
    public Task<IEnumerable<WorkflowStageGetModel>> GetAllStagesOfTheBoardAsync(int boardId);
    public Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostModel model);
    public Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId);
    public Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePutModel model);
    public Task MoveStage(int boardId, int stageId, bool isMovingForward);
    public Task DeleteStageAsync(int boardId, int stageId);
}
