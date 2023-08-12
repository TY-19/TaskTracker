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
        List<Board?> boards = await GetAllBoardsFromTheDB().ToListAsync();
        return _mapper.Map<List<BoardGetModel>>(boards);
    }

    public async Task<IEnumerable<BoardGetModel>> GetBoardOfTheEmployeeAsync(string userName)
    {
        UserProfileModel? user = await _userService.GetUserByNameOrIdAsync(userName);
        if (user == null)
            return new List<BoardGetModel>();

        bool hasAccessToAllBoards = user.Roles.Contains(DefaultRolesNames.DEFAULT_ADMIN_ROLE) ||
            user.Roles.Contains(DefaultRolesNames.DEFAULT_MANAGER_ROLE);

        return hasAccessToAllBoards
            ? await GetAllBoardsAsync()
            : await GetBoardAccessibleToTheEmployeeAsync(userName);
    }

    private async Task<IEnumerable<BoardGetModel>> GetBoardAccessibleToTheEmployeeAsync(string userName)
    {
        List<Board?> boards = await GetAllBoardsFromTheDB()
            .Where(b => b!.Employees
                .Select(e => e.User)
                .Where(u => u != null)
                .Select(u => u!.UserName)
                .Contains(userName))
            .ToListAsync();

        return _mapper.Map<List<BoardGetModel>>(boards);
    }

    private IQueryable<Board?> GetAllBoardsFromTheDB()
    {
        return _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .AsNoTracking();
    }

    public async Task<BoardGetModel?> GetBoardByIdAsync(int id)
    {
        return _mapper.Map<BoardGetModel>(await GetBoardFromDbByIdAsync(id));
    }
    private async Task<Board?> GetBoardFromDbByIdAsync(int id)
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
        Board? board = await _context.Boards
            .Include(b => b.Stages)
            .Include(b => b.Assignments).ThenInclude(a => a.Subparts)
            .Include(b => b.Assignments).ThenInclude(a => a.Stage)
            .Include(b => b.Employees).ThenInclude(e => e.User)
            .FirstOrDefaultAsync(e => e.Name == name);
        return _mapper.Map<BoardGetModel>(board);
    }

    public async Task<BoardGetModel?> AddBoardAsync(string name)
    {
        await EnsureTheNameIsAllowed(name);
        await _context.Boards.AddAsync(new Board() { Name = name });
        await _context.SaveChangesAsync();
        return await GetBoardByNameAsync(name);
    }

    private async Task EnsureTheNameIsAllowed(string name)
    {
        if (string.IsNullOrEmpty(name) || int.TryParse(name, out _))
            throw new ArgumentException("BoardName can't be empty or contains only digits", nameof(name));

        if (await GetBoardByNameAsync(name) != null)
            throw new ArgumentException($"Board with the name {name} has already exist", nameof(name));
    }

    public async Task UpdateBoardNameAsync(int id, string? newName)
    {
        if (newName != null)
        {
            await EnsureTheNameIsAllowed(newName);
            Board board = await GetBoardFromDbByIdAsync(id)
                ?? throw new ArgumentException("Board does not exist");
            board.Name = newName;
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBoardAsync(int id)
    {
        Board? board = await _context.Boards.FindAsync(id);
        if (board != null)
        {
            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
        }
    }
}
