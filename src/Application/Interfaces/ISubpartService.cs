using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface ISubpartService
{
    public Task<IEnumerable<SubpartGetModel>> GetAllSubpartOfTheAssignmentAsync(int assignmentId);
    public Task<SubpartGetModel?> GetSubpartByIdAsync(int assignmentId, int subpartId);
    public Task<SubpartGetModel?> AddSubpartToTheAssignmentAsync(SubpartPostPutModel model);
    public Task UpdateSubpartAsync(int assignmentId, int subpartId, SubpartPostPutModel model);
    public Task DeleteSubpartAsync(int assignmentId, int subpartId);
}
