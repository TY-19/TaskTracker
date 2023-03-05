using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    public AssignmentService(ITrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AssignmentGetModel>> GetAllAssignmentsOfTheBoardAsync(int boardId)
    {
        var assignments = await _context.Assignments
                .Where(a => a.BoardId == boardId)
                .Include(a => a.Subparts)
                .Include(a => a.ResponsibleEmployee)
                .Include(a => a.Stage)
                .AsNoTracking()
                .ToListAsync();
        var mapped = _mapper.Map<List<AssignmentGetModel>>(assignments);
        return mapped;
    }

    public async Task<AssignmentGetModel?> CreateAssignmentAsync(int boardId, AssignmentPostPutModel assignmentModel)
    {
        if (assignmentModel == null)
        {
            throw new ArgumentException($"Incorrect assignment", nameof(assignmentModel));
        }

        if ((await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId)) == null)
        {
            throw new ArgumentException($"Incorrect board to create assignment", nameof(boardId));
        }

        return await CreateAssignmentInternalAsync(boardId, assignmentModel);
    }

    private async Task<AssignmentGetModel?> CreateAssignmentInternalAsync(int boardId, AssignmentPostPutModel assignmentModel)
    {
        var assignment = _mapper.Map<Assignment>(assignmentModel);
        assignment.BoardId = boardId;
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
        var mapped = _mapper.Map<AssignmentGetModel>(assignment);
        return mapped;
    }

    public async Task<AssignmentGetModel?> GetAssignmentAsync(int boardId, int taskId)
    {
        var assignment = await GetAssignmentInnerAsync(taskId);

        if (assignment == null || assignment.BoardId != boardId)
            return null;

        var mapped = _mapper.Map<AssignmentGetModel>(assignment);

        return mapped;
    }

    private async Task<Assignment?> GetAssignmentInnerAsync(int taskId)
    {
        return await _context.Assignments
            .Include(a => a.Subparts)
            .Include(a => a.Board)
            .Include(a => a.ResponsibleEmployee)
            .Include(a => a.Stage)
            .FirstOrDefaultAsync(a => a.Id == taskId);
    }

    public async Task UpdateAssignmentAsync(int boardId, int taskId, AssignmentPostPutModel model)
    {
        var assignment = await GetAssignmentInnerAsync(taskId);

        if (assignment == null || assignment.BoardId != boardId)
            throw new ArgumentException("There is no assignment with the id on the board");

        _mapper.Map(model, assignment);

        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAssignmentAsync(int boardId, int taskId)
    {
        var toDelete = await GetAssignmentInnerAsync(taskId);
        if (toDelete == null || toDelete.BoardId != boardId)
            return;
        _context.Assignments.Remove(toDelete);
        await _context.SaveChangesAsync();
    }
}
