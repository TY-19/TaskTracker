using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class SubpartService : ISubpartService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    public SubpartService(ITrackerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<SubpartGetModel?> AddSubpartToTheAssignmentAsync(SubpartPostPutModel model)
    {
        if ((await _context.Assignments.FirstOrDefaultAsync(b => b.Id == model.AssignmentId)) == null)
        {
            throw new ArgumentException($"An incorrect assignment to add subpart");
        }
        var subpart = _mapper.Map<Subpart>(model);
        await _context.Subparts.AddAsync(subpart);
        await _context.SaveChangesAsync();
        return _mapper.Map<SubpartGetModel>(subpart);
    }

    public async Task DeleteSubpartAsync(int assignmentId, int subpartId)
    {
        var subpart = await _context.Subparts.FirstOrDefaultAsync(
            s => s.Id == subpartId && s.AssignmentId == assignmentId);
        if (subpart == null)
            return;

        _context.Subparts.Remove(subpart);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SubpartGetModel>> GetAllSubpartOfTheAssignmentAsync(int assignmentId)
    {
        var subparts = await _context.Subparts
            .Where(s => s.AssignmentId == assignmentId)
            .ToListAsync();
        var mapped = _mapper.Map<List<SubpartGetModel>>(subparts);
        return mapped;
    }

    public async Task<SubpartGetModel?> GetSubpartByIdAsync(int assignmentId, int subpartId)
    {
        var subpart = await _context.Subparts.FirstOrDefaultAsync(
            s => s.Id == subpartId && s.AssignmentId == assignmentId);
        var mapped = _mapper.Map<SubpartGetModel>(subpart);
        return mapped;
    }

    public async Task UpdateSubpartAsync(int assignmentId, int subpartId, SubpartPostPutModel model)
    {
        var subpart = await _context.Subparts.FirstOrDefaultAsync(
            s => s.Id == subpartId && s.AssignmentId == assignmentId);
        if (subpart == null)
            throw new ArgumentException("There are no such a subpart in the assignment");

        _mapper.Map(model, subpart);
        await _context.SaveChangesAsync();
    }
}
