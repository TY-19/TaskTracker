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
        var mapped = _mapper.Map<List<WorkflowStageGetModel>>(stages);
        return mapped;
    }

    public async Task<WorkflowStageGetModel> AddStageToTheBoardAsync(int boardId, WorkflowStagePostPutModel model)
    {
        var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null)
        {
            throw new ArgumentException("The board with such an id does not exist", nameof(boardId));
        }
        var stage = new WorkflowStage()
        {
            Name = model.Name,
            BoardId = boardId,
            Position = ((await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId))?
                .Stages?.Select(s => s.Position).DefaultIfEmpty().Max() ?? 0) + 1,
        };
        _context.Stages.Add(stage);
        board.Stages.Add(stage);
        await _context.SaveChangesAsync();
        var mapped = _mapper.Map<WorkflowStageGetModel>(stage);
        return mapped;
    }

    public async Task<WorkflowStageGetModel?> GetStageByIdAsync(int boardId, int stageId)
    {
        var stage = await _context.Stages
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);
        var mapped = _mapper.Map<WorkflowStageGetModel>(stage);
        return mapped;
    }

    public async Task UpdateStageAsync(int boardId, int stageId, WorkflowStagePostPutModel model)
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
            .FirstOrDefaultAsync(s => s.BoardId == boardId && s.Id == stageId);
        if (stage == null)
            return;

        _context.Stages.Remove(stage);
        await _context.SaveChangesAsync();
    }
}
