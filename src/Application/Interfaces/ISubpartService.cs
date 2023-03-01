using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface ISubpartService
{
    public Task<IEnumerable<SubpartGetModel>> GetAllSubpartOfTheAssignmentAsync(int assignmentId);
    public Task<SubpartGetModel?> GetSubpartByIdAsync(int subpartId);
    public Task<SubpartGetModel?> AddSubpartToTheAssignmentAsync(SubpartPostPutModel model);
    public Task UpdateSubpartAsync(int subpartId, SubpartPostPutModel model);
    public Task DeleteSubpartAsync(int subpartId);
}
