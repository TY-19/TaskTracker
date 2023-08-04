using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure;

/// <summary>
/// Contains configuration and all required DbSets to work with database
/// </summary>
public class TrackerDbContext : IdentityDbContext<User>, ITrackerDbContext
{
    public TrackerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Assignment>()
            .HasOne(a => a.Stage)
            .WithMany(ws => ws.Assignments)
            .HasForeignKey(a => a.StageId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }

    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Subpart> Subparts => Set<Subpart>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<WorkflowStage> Stages => Set<WorkflowStage>();
    public DbSet<Board> Boards => Set<Board>();
}
