using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IStageService
{
    public Task<IEnumerable<WorkflowStageGetModel>> GetAllStagesOfTheBoardAsync(int boardId);
    public Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostPutModel model);
    public Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId);
    public Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePostPutModel model);
    public Task DeleteStageAsync(int boardId, int stageId);
}
