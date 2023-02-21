using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface ITrackerDbContext
{
    public DbSet<Assignment> Assignments { get; }
    public DbSet<Subpart> Subparts { get; }
    public DbSet<Employee> Employees { get; }
    public DbSet<WorkflowStage> Stages { get; }
    public DbSet<Board> Boards { get; }
    public Task SaveChangesAsync();
}
