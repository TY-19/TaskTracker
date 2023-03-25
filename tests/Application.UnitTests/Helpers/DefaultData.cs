using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Helpers;

internal static class DefaultData
{
    public static async Task SeedAsync(TestDbContext context)
    {
        await context.Boards.AddRangeAsync(Board1, Board2);
        await context.Stages.AddRangeAsync(Stage1, Stage2, Stage3);
        await context.Assignments.AddRangeAsync(Assignment1, Assignment2, Assignment3);
        await context.Employees.AddRangeAsync(Employee1, Employee2);
        await context.Users.AddRangeAsync(User1, User2);
        await context.SaveChangesAsync();
        await SetManyToManyRelationshipsAsync(context);
    }
    private static async Task SetManyToManyRelationshipsAsync(TestDbContext context)
    {
        var employyes = await context.Employees.ToListAsync();
        var board1 = await context.Boards.FirstOrDefaultAsync(b => b.Id == 1);
        board1?.Employees.Add(employyes[0]);
        board1?.Employees.Add(employyes[1]);

        await context.SaveChangesAsync();
    }
    private static Board Board1 => new()
    {
        Id = 1,
        Name = "Board1"
    };
    private static Board Board2 => new()
    {
        Id = 2,
        Name = "Board2"
    };
    private static WorkflowStage Stage1 => new()
    {
        Id = 1,
        Name = "Start",
        Position = 1,
        BoardId = Board1.Id,
    };
    private static WorkflowStage Stage2 => new()
    {
        Id = 2,
        Name = "Finish",
        Position = 2,
        BoardId = Board1.Id,
    };
    private static WorkflowStage Stage3 => new()
    {
        Id = 3,
        Name = "Only",
        Position = 1,
        BoardId = Board2.Id,
    };
    private static Subpart Subpart1 => new()
    {
        Id = 1,
        AssignmentId = 1,
        Name = "Part 1",
        Description = "The first part of the first task"
    };
    private static Subpart Subpart2 => new()
    {
        Id = 2,
        AssignmentId = 1,
        Name = "Part 2",
        Description = "The second part of the first task"
    };
    private static Subpart Subpart3 => new()
    {
        Id = 3,
        AssignmentId = 2,
        Name = "Part 1",
        Description = "The first part of the second task"
    };
    private static Subpart Subpart4 => new()
    {
        Id = 4,
        AssignmentId = 2,
        Name = "Part 2",
        Description = "The second part of the second task"
    };
    private static Assignment Assignment1 => new()
    {
        Id = 1,
        Topic = "First Assignment",
        Description = "Awesome description",
        Deadline = new DateTime(2023, 04, 01),
        StageId = 1,
        BoardId = 1,
        Subparts = { Subpart1, Subpart2 },
        ResponsibleEmployeeId = 1
    };
    private static Assignment Assignment2 => new()
    {
        Id = 2,
        Topic = "Second Assignment",
        Description = "Other description",
        Deadline = new DateTime(2023, 04, 02),
        StageId = 2,
        BoardId = 1,
        Subparts = { Subpart3, Subpart4 },
        ResponsibleEmployeeId = 1
    };
    private static Assignment Assignment3 => new()
    {
        Id = 3,
        Topic = "Third Assignment",
        Description = "The third one",
        Deadline = new DateTime(2023, 04, 03),
        StageId = 3,
        BoardId = 2,
        Subparts = new List<Subpart>(),
        ResponsibleEmployeeId = 2
    };

    private static Employee Employee1 => new()
    {
        Id = 1,
        FirstName = "Test",
        LastName = "Employee",
    };

    private static Employee Employee2 => new()
    {
        Id = 2,
        FirstName = "Second",
        LastName = "Worker",
    };

    private static User User1 => new()
    {
        Id = "12345678-1234-1234-1234-123456789012",
        UserName = "testUser",
        NormalizedUserName = "testUser",
        Email = "test@email.com",
        EmployeeId = 1,
        PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8"
    };

    private static User User2 => new()
    {
        Id = "abcdefgh-abcd-abcd-abcd-abcdefgh",
        UserName = "secondUser",
        NormalizedUserName = "secondUser",
        Email = "test2@email.com",
        EmployeeId = 2,
        PasswordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8"
    };
}
