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
        var stages = await _context.Stages
            .Where(s => s.BoardId == boardId)
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<WorkflowStageGetModel>>(stages);
    }

    public async Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostModel model)
    {
        var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);

        if (board == null)
            throw new ArgumentException("The board with such an id does not exist", nameof(boardId));

        var stage = await CreateStage(model, boardId);
        board.Stages.Add(stage);
        await _context.SaveChangesAsync();
        return _mapper.Map<WorkflowStageGetModel>(stage);
    }

    private async Task<WorkflowStage> CreateStage(WorkflowStagePostModel model, int boardId)
    {
        return new WorkflowStage()
        {
            Name = model.Name,
            BoardId = boardId,
            Position = ((await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId))?
                .Stages?.Select(s => s.Position).DefaultIfEmpty().Max() ?? 0) + 1,
        };
    }

    public async Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId)
    {
        var stage = await _context.Stages
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);
        return _mapper.Map<WorkflowStageGetModel>(stage);
    }

    public async Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePutModel model)
    {
        var stage = await _context.Stages
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);

        if (stage == null)
            throw new ArgumentException("There is no stage with the id on the board");

        _mapper.Map(model, stage);

        _context.Stages.Update(stage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStageAsync(int boardId, int stageId)
    {
        var stage = await _context.Stages
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
        var otherStagesPositions = _context.Stages
            .Where(s => s.BoardId == stage.BoardId).Select(s => s.Position);

        int leftPosition = otherStagesPositions.Where(p => p < stage.Position).DefaultIfEmpty().Max();
        int rightPosition = otherStagesPositions.Where(p => p > stage.Position).DefaultIfEmpty().Min();
        int newPosition = leftPosition != 0 ? leftPosition : rightPosition;

        if (newPosition == 0)
            throw new InvalidOperationException(
                "Stage cannot be deleted because it contains assignments and there are no other stages on the board to transfer them");

        await _context.Assignments
            .Where(a => a.BoardId == stage.BoardId && a.StageId == stage.Id)
            .ForEachAsync(a => a.StageId = newPosition);
    }
}
