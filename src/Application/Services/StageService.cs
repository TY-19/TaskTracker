using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class StageService : IStageService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    public StageService(ITrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<WorkflowStageGetModel>> GetAllStagesOfTheBoardAsync(int boardId)
    {
        List<WorkflowStage> stages = await _context.Stages
            .Where(s => s.BoardId == boardId)
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<WorkflowStageGetModel>>(stages);
    }

    public async Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostModel model)
    {
        Board board = await _context.Boards
            .Include(b => b.Stages)
            .FirstOrDefaultAsync(b => b.Id == boardId)
            ?? throw new ArgumentException("The board with such an id does not exist", nameof(boardId));

        WorkflowStage stage = CreateStage(model, board);
        board.Stages.Add(stage);
        await _context.SaveChangesAsync();
        return _mapper.Map<WorkflowStageGetModel>(stage);
    }

    private static WorkflowStage CreateStage(WorkflowStagePostModel model, Board board)
    {
        return new WorkflowStage()
        {
            Name = model.Name,
            BoardId = board.Id,
            Position = (board.Stages?
                .Select(s => s.Position)
                .DefaultIfEmpty().Max() ?? 0) + 1,
        };
    }

    public async Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId)
    {
        WorkflowStage? stage = await _context.Stages
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);
        return _mapper.Map<WorkflowStageGetModel>(stage);
    }

    public async Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePutModel model)
    {
        WorkflowStage stage = await _context.Stages
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId)
            ?? throw new ArgumentException("There is no stage with the id on the board");

        _mapper.Map(model, stage);
        _context.Stages.Update(stage);
        await _context.SaveChangesAsync();
    }

    public async Task MoveStage(int boardId, int stageId, bool isMovingForward)
    {
        WorkflowStage? stageToMove = await _context.Stages
            .FirstOrDefaultAsync(s => s.Id == stageId && s.BoardId == boardId)
            ?? throw new ArgumentException("There is no stage with the id on the board");

        int newPosition = await GetNewPositionAsync(stageToMove, isMovingForward);
        await DisplaceStageAtNewPosition(boardId, stageToMove, newPosition);
        stageToMove.Position = newPosition;
        _context.Stages.Update(stageToMove);
        await _context.SaveChangesAsync();
    }

    private async Task<int> GetNewPositionAsync(WorkflowStage stageToMove, bool isMovingForward)
    {
        int newPosition = isMovingForward
            ? stageToMove.Position + 1
            : stageToMove.Position - 1;

        if (newPosition == 0 || newPosition > await _context.Stages.MaxAsync(s => s.Position))
            throw new ArgumentException($"Stage is already on the {(isMovingForward ? "last" : "first")} position and can't be moved");

        return newPosition;
    }
    private async Task DisplaceStageAtNewPosition(int boardId,
        WorkflowStage stageToMove, int displaceFromPosition)
    {
        WorkflowStage? stageToDisplace = await _context.Stages
            .FirstOrDefaultAsync(s=> s.BoardId == boardId && s.Position == displaceFromPosition);

        if(stageToDisplace != null)
        {
            stageToDisplace.Position = stageToMove.Position;
            _context.Stages.Update(stageToDisplace);
        }
    }

    public async Task DeleteStageAsync(int boardId, int stageId)
    {
        WorkflowStage? stage = await _context.Stages
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);
        if (stage == null)
            return;
        if (stage.Assignments.Any())
            await MoveAssignmentsToTheOtherStage(stage);

        _context.Stages.Remove(stage);
        await _context.SaveChangesAsync();
    }

    private async Task MoveAssignmentsToTheOtherStage(WorkflowStage stage)
    {
        int newPosition = GetDestinationStagePosition(stage);
        int newStageId = await GetIdOfStageInThePositionAsync(stage.BoardId, newPosition);

        await _context.Assignments
            .Where(a => a.BoardId == stage.BoardId && a.StageId == stage.Id)
            .ForEachAsync(a => a.StageId = newStageId);
    }

    private int GetDestinationStagePosition(WorkflowStage currentStage)
    {
        var otherStagesPositions = _context.Stages
            .Where(s => s.BoardId == currentStage.BoardId)
            .Select(s => s.Position);

        int leftPosition = otherStagesPositions
            .Where(p => p < currentStage.Position)
            .DefaultIfEmpty()
            .Max();
        int rightPosition = otherStagesPositions
            .Where(p => p > currentStage.Position)
            .DefaultIfEmpty()
            .Min();
        int newPosition = leftPosition == 0 ? rightPosition : leftPosition;

        if (newPosition == 0)
            throw new InvalidOperationException("Stage cannot be deleted because it contains assignments and there are no other stages on the board to transfer them");

        return newPosition;
    }

    private async Task<int> GetIdOfStageInThePositionAsync(int boardId, int position)
    {
        int newStageId = (await _context.Stages.FirstOrDefaultAsync(
            s => s.BoardId == boardId && s.Position == position))?.Id ?? 0;

        if (newStageId == 0)
            throw new InvalidOperationException("Stage cannot be deleted because it contains assignments and there are no other stages on the board to transfer them");

        return newStageId;
    }
}
