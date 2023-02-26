using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class BoardService : IBoardService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    public BoardService(ITrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<BoardGetModel> GetAllBoards()
    {
        var boards = _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees)
            .AsNoTracking();
        var mapped = _mapper.Map<List<BoardGetModel>>(boards);
        return mapped;
    }

    public async Task<BoardGetModel?> GetBoardByIdAsync(int id)
    {
        var board = await GetBoardByIdInnerAsync(id);

        var mapped = _mapper.Map<BoardGetModel>(board);
        return mapped;
    }
    private async Task<Board?> GetBoardByIdInnerAsync(int id)
    {
        return await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<BoardGetModel?> GetBoardByNameAsync(string name)
    {
        var board = await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(e => e.Name == name);
        var mapped = _mapper.Map<BoardGetModel>(board);
        return mapped;
    }

    public async Task<BoardGetModel?> AddBoardAsync(string name)
    {
        if (name == null || await GetBoardByNameAsync(name) != null)
        {
            throw new ArgumentException(
                $"Board with the name {name} has already exist", nameof(name));
        }
        await _context.Boards.AddAsync(new Board() { Name = name });
        await _context.SaveChangesAsync();
        return await GetBoardByNameAsync(name);
    }

    public async Task UpdateBoardNameAsync(int id, string newName)
    {
        var board = await GetBoardByIdInnerAsync(id);
        if (newName == null || board == null)
            return;

        board.Name = newName;
        _context.Boards.Update(board);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBoardAsync(int id)
    {
        var board = await _context.Boards.FindAsync(id);
        if (board != null)
        {
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<AssignmentGetModel>> GetAllAssignmentsOfTheBoard(int boardId)
    {
        var assignments = await _context.Assignments
                .Where(a => a.BoardId == boardId)
                .Include(a => a.Subparts).ThenInclude(sp => sp.Stage)
                .Include(a => a.ResponsibleEmployee)
                .Include(a => a.Stage)
                .AsNoTracking()
                .ToListAsync();
        var mapped = _mapper.Map<List<AssignmentGetModel>>(assignments);
        return mapped;
    }

    public Task<AssignmentGetModel?> CreateAssignmentAsync(int boardId, AssignmentPostPutModel assignmentModel)
    {
        if (assignmentModel == null)
        {
            throw new ArgumentException($"Incorrect assignment", nameof(assignmentModel));
        }

        if (GetBoardByIdAsync(boardId) == null)
        {
            throw new ArgumentException($"Incorrect board to create assignment", nameof(boardId));
        }

        return CreateAssignmentInternalAsync(boardId, assignmentModel);
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
            .Include(a => a.Subparts).ThenInclude(sp => sp.Stage)
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
        var stage = new WorkflowStage()
        {
            Name = model.Name,
            BoardId = boardId,
            Position = ((await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId))?
                .Stages?.Select(s => s.Position).DefaultIfEmpty().Max() ?? 0) + 1,
        };
        _context.Stages.Add(stage);
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

    public async Task<IEnumerable<EmployeeGetBoardModel>> GetAllEmployeeAsync(int? boardId)
    {
        IEnumerable<Employee> employees;
        if (boardId == null)
        {
            employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }
        else
        {
            employees = await _context.Employees
                .Where(s => s.Boards.Select(b => b.Id).Contains(boardId ?? -1))
                .AsNoTracking()
                .ToListAsync();
        }
        
        var mapped = _mapper.Map<List<EmployeeGetBoardModel>>(employees);
        return mapped;
    }

    public async Task<EmployeeGetBoardModel> GetEmployeeById(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
        var mapped = _mapper.Map<EmployeeGetBoardModel>(employee);
        return mapped;
    }

    public async Task AddEmployeeToTheBoardAsync(int boardId, UserProfileModel user)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => 
            e.User != null && e.User.Id == user.Id);
        if (employee == null)
        {
            employee = new Employee
            {
                User = new User { Id = user.Id }
            };
            if (user.EmployeeId != null)
                employee.Id = (int)user.EmployeeId;
            if (user.FirstName != null)
                employee.FirstName = user.FirstName;
            if (user.LastName != null)
                employee.LastName = user.LastName;
            employee.Boards = new List<Board>();
        }
        var board = await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null)
            throw new ArgumentException("Board with a such id does not exist");

        employee.Boards.Add(board);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmployeeFromTheBoard(int boardId, int employeeId)
    {
        var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);
        var board = await _context.Boards
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (employee == null || board == null)
            return;

        board.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}