using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Helpers;

internal class TestDbContext : IdentityDbContext, ITrackerDbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Subpart>()
            .Property(s => s.PercentValue)
            .HasColumnType("decimal(8, 2)");
        builder.Entity<Assignment>()
            .HasOne(a => a.Stage)
            .WithMany(ws => ws.Assignments)
            .HasForeignKey(a => a.StageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Subpart> Subparts => Set<Subpart>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<WorkflowStage> Stages => Set<WorkflowStage>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<User> Users => Set<User>();

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}
