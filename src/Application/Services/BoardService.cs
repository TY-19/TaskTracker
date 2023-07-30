using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class BoardService : IBoardService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    public BoardService(ITrackerDbContext context,
        IMapper mapper,
        IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<IEnumerable<BoardGetModel>> GetAllBoardsAsync()
    {
        var boards = await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<BoardGetModel>>(boards);
    }

    public async Task<IEnumerable<BoardGetModel>> GetBoardOfTheEmployeeAsync(string userName)
    {
        var user = await _userService.GetUserByNameOrIdAsync(userName);
        if (user == null)
            return new List<BoardGetModel>();
        if (user.Roles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE) ||
            user.Roles.Contains(DefaultRolesNames.DEFAULT_MANAGER_ROLE))
        {
            return await GetAllBoardsAsync();
        }

        var boards = await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .AsNoTracking()
            .Where(b => b.Employees
                .Select(e => e.User)
                .Where(u => u != null)
                .Select(u => u!.UserName)
                .Contains(userName))
            .ToListAsync();

        return _mapper.Map<List<BoardGetModel>>(boards);
    }

    public async Task<BoardGetModel?> GetBoardByIdAsync(int id)
    {
        var board = await GetBoardByIdInnerAsync(id);

        return _mapper.Map<BoardGetModel>(board);
    }
    private async Task<Board?> GetBoardByIdInnerAsync(int id)
    {
        return await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<BoardGetModel?> GetBoardByNameAsync(string name)
    {
        var board = await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .FirstOrDefaultAsync(e => e.Name == name);
        return _mapper.Map<BoardGetModel>(board);
    }

    public async Task<BoardGetModel?> AddBoardAsync(string name)
    {
        if (string.IsNullOrEmpty(name) || int.TryParse(name, out _))
            throw new ArgumentException(
                "BoardName can't be empty or contains only digits", nameof(name));

        if (await GetBoardByNameAsync(name) != null)
            throw new ArgumentException(
                $"Board with the name {name} has already exist", nameof(name));

        await _context.Boards.AddAsync(new Board() { Name = name });
        await _context.SaveChangesAsync();
        return await GetBoardByNameAsync(name);
    }

    public async Task UpdateBoardNameAsync(int id, string? newName)
    {
        var board = await GetBoardByIdInnerAsync(id);
        if (string.IsNullOrEmpty(newName) || int.TryParse(newName, out _) || board == null)
            throw new ArgumentException(
                $"The board name cannot be updated", nameof(newName));

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
}