using AutoMapper;
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
}