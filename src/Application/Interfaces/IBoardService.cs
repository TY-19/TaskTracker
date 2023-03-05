using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IBoardService
{
    public Task<IEnumerable<BoardGetModel>> GetAllBoardsAsync();
    public Task<BoardGetModel?> GetBoardByIdAsync(int id);
    public Task<BoardGetModel?> GetBoardByNameAsync(string name);
    public Task<BoardGetModel?> AddBoardAsync(string name);
    public Task UpdateBoardNameAsync(int id, string newName);
    public Task DeleteBoardAsync(int id);
}
