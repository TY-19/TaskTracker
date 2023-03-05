using AutoMapper;
using TaskTracker.Application.Mapping;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Mapping;

public class AutomapperProfileTests
{
    [Fact]
    public void Mapper_MapsUserToUserProfile()
    {
        IMapper mapper = GetMapper();
        User user = user1;

        var result = mapper.Map<UserProfileModel>(user);

        Assert.Multiple(
            () => Assert.Equal(user.UserName, result.UserName),
            () => Assert.Equal(user.UserName, result.UserName),
            () => Assert.Equal(user.Email, result.Email),
            () => Assert.Equal(user.EmployeeId, result.EmployeeId),
            () => Assert.Equal(user.Employee?.FirstName, result.FirstName),
            () => Assert.Equal(user.Employee?.LastName, result.LastName),
            () => Assert.Equal(2, result.BoardsIds.Count()),
            () => Assert.Equal(2, result.AssignmentsIds.Count())
        );
    }

    [Fact]
    public void Mapper_MapsSubpartToSubpartGetModel()
    {
        IMapper mapper = GetMapper();
        Subpart subpart = subpart1;

        var result = mapper.Map<SubpartGetModel>(subpart);

        Assert.Multiple(
            () => Assert.Equal(subpart.Id, result.Id),
            () => Assert.Equal(subpart.Name, result.Name),
            () => Assert.Equal(subpart.AssignmentId, result.AssignmentId)
        );
    }

    [Fact]
    public void Mapper_MapsSubpartPostPutModelToSubpart()
    {
        IMapper mapper = GetMapper();
        SubpartPostPutModel subpartmodel = new()
        {
            Name = "Test",
            Description = "Description",
            PercentValue = 50,
            AssignmentId = 1
        };

        var result = mapper.Map<Subpart>(subpartmodel);

        Assert.Multiple(
            () => Assert.Equal(subpartmodel.Name, result.Name),
            () => Assert.Equal(subpartmodel.Description, result.Description),
            () => Assert.Equal(subpartmodel.PercentValue, result.PercentValue),
            () => Assert.Equal(subpartmodel.AssignmentId, result.AssignmentId)
        );
    }

    [Fact]
    public void Mapper_MapsAssignmentToAssignmentGetModel()
    {
        IMapper mapper = GetMapper();
        Assignment assignment = assignment1;

        var result = mapper.Map<AssignmentGetModel>(assignment);

        Assert.Multiple(
            () => Assert.Equal(assignment.Id, result.Id),
            () => Assert.Equal(assignment.Topic, result.Topic),
            () => Assert.Equal(assignment.Description, result.Description),
            () => Assert.False(result.IsCompleted),
            () => Assert.Equal(assignment.BoardId, result.BoardId),
            () => Assert.Equal(assignment.StageId, result.StageId),
            () => Assert.Equal(assignment.ResponsibleEmployeeId, result.ResponsibleEmployeeId),
            () => Assert.Equal(2, result.Subparts.Count),
            () => Assert.Equal(assignment.Subparts.FirstOrDefault()?.Name,
                result.Subparts.FirstOrDefault()?.Name)
        );
    }

    [Fact]
    public void Mapper_MapsAssignmentPostPutModelToAssignment()
    {
        IMapper mapper = GetMapper();
        AssignmentPostPutModel model = new()
        {
            Topic = "Topic 1",
            Description = "Description",
            Deadline = new DateTime(2023, 4, 1),
            StageId = 1,
            ResponsibleEmployeeId = 1
        };

        var result = mapper.Map<Assignment>(model);

        Assert.Multiple(
            () => Assert.Equal(model.Topic, result.Topic),
            () => Assert.Equal(model.Description, result.Description),
            () => Assert.Equal(model.Deadline, result.Deadline),
            () => Assert.Equal(model.StageId, result.StageId),
            () => Assert.Equal(model.ResponsibleEmployeeId, result.ResponsibleEmployeeId)
        );
    }

    [Fact]
    public void Mapper_MapsEmployeeToEmployeeGetBoardModel()
    {
        IMapper mapper = GetMapper();
        Employee employye = employee1;

        var result = mapper.Map<EmployeeGetBoardModel>(employye);

        Assert.Multiple(
            () => Assert.Equal(employye.Id, result.Id),
            () => Assert.Equal(employye.FirstName, result.FirstName),
            () => Assert.Equal(employye.LastName, result.LastName)
        );
    }

    [Fact]
    public void Mapper_MapsEmployeePostPutModelToEmployee()
    {
        IMapper mapper = GetMapper();
        EmployeePostPutModel model = new()
        {
            FirstName = "First",
            LastName = "Last"
        };

        var result = mapper.Map<Employee>(model);

        Assert.Multiple(
            () => Assert.Equal(model.FirstName, result.FirstName),
            () => Assert.Equal(model.LastName, result.LastName)
        );
    }

    [Fact]
    public void Mapper_MapsWorkflowStageToWorkflowStageGetModel()
    {
        IMapper mapper = GetMapper();
        WorkflowStage stage = stage1;

        var result = mapper.Map<WorkflowStageGetModel>(stage);

        Assert.Multiple(
            () => Assert.Equal(stage.Id, result.Id),
            () => Assert.Equal(stage.Name, result.Name),
            () => Assert.Equal(stage.Position, result.Position),
            () => Assert.Equal(stage.BoardId, result.BoardId)
        );
    }

    [Fact]
    public void Mapper_MapsWorkflowStagePostPutModelToWorkflowStage()
    {
        IMapper mapper = GetMapper();
        WorkflowStagePostPutModel model = new()
        {
            Name = "Stage1"
        };

        var result = mapper.Map<WorkflowStage>(model);

        Assert.Equal(model.Name, result.Name);
    }

    [Fact]
    public void Mapper_MapsWorkflowStagePostPutToExistedWorkflowStage()
    {
        IMapper mapper = GetMapper();
        WorkflowStage stage = new()
        {
            Id = 1,
            Name = "Original",
            Position = 1,
        };
        WorkflowStagePostPutModel model = new()
        {
            Name = "Mapped"
        };

        mapper.Map(model, stage);

        Assert.Multiple(
            () => Assert.Equal("Mapped", stage.Name),
            () => Assert.Equal(1, stage.Id),
            () => Assert.Equal(1, stage.Position)
        );
    }

    [Fact]
    public void Mapper_MapsBoardToBoardGetModel()
    {
        IMapper mapper = GetMapper();
        Board board = new()
        {
            Id = 1,
            Name = "Board1",
            Stages = new List<WorkflowStage>() { stage1, stage2 },
            Assignments = new List<Assignment> { assignment1 },
            Employees = new List<Employee>() { employee1 }
        };

        var result = mapper.Map<BoardGetModel>(board);

        Assert.Multiple(
            () => Assert.Equal(board.Id, result.Id),
            () => Assert.Equal(board.Name, result.Name),
            () => Assert.Equal(2, result.Stages.Count),
            () => Assert.Equal(board.Stages.FirstOrDefault()?.Name,
                result.Stages.FirstOrDefault()?.Name),
            () => Assert.Equal(board.Assignments.FirstOrDefault()?.Topic,
                result.Assignments.FirstOrDefault()?.Topic),
            () => Assert.Equal(board.Employees.FirstOrDefault()?.LastName,
                result.Employees.FirstOrDefault()?.LastName)
        );
    }

    [Fact]
    public void Mapper_MapsBoardPostPutModelToBoard()
    {
        IMapper mapper = GetMapper();
        BoardPostPutModel model = new() { Name = "NewName" };

        var result = mapper.Map<Board>(model);

        Assert.Equal(model.Name, result.Name);
    }

    [Fact]
    public void Mapper_MapsBoardPostPutModelToExistedBoard()
    {
        IMapper mapper = GetMapper();
        Board board = new()
        {
            Id = 5,
            Name = "Board1",
        };
        BoardPostPutModel model = new() { Name = "NewName" };

        mapper.Map(model, board);

        Assert.Multiple(
            () => Assert.Equal(5, board.Id),
            () => Assert.Equal(model.Name, board.Name)
        );
    }

    private static IMapper GetMapper()
    {
        return new MapperConfiguration(cfg =>
            cfg.AddProfile<AutomapperProfile>()).CreateMapper();
    }


    private readonly static Board board1 = new() { Id = 1, Name = "Board1" };
    private readonly static Board board2 = new() { Id = 2, Name = "Board2" };
    private readonly static WorkflowStage stage1 = new()
    {
        Id = 1,
        Name = "Start",
        Position = 1,
        BoardId = board1.Id,
        Board = board1,
    };
    private readonly static WorkflowStage stage2 = new()
    {
        Id = 2,
        Name = "Finish",
        Position = 2,
        BoardId = board1.Id,
        Board = board1,
    };
    private readonly static Subpart subpart1 = new()
    {
        Id = 1,
        AssignmentId = 1,
        Name = "Part 1",
        Description = "The first part of the first task"
    };
    private readonly static Subpart subpart2 = new()
    {
        Id = 2,
        AssignmentId = 1,
        Name = "Part 2",
        Description = "The second part of the first task"
    };
    private readonly static Subpart subpart3 = new()
    {
        Id = 3,
        AssignmentId = 2,
        Name = "Part 1",
        Description = "The first part of the second task"
    };
    private readonly static Subpart subpart4 = new()
    {
        Id = 4,
        AssignmentId = 2,
        Name = "Part 2",
        Description = "The second part of the second task"
    };
    private readonly static Assignment assignment1 = new()
    {
        Id = 1,
        Topic = "First Assignment",
        Description = "Awesome description",
        Deadline = new DateTime(2023, 04, 01),
        StageId = 1,
        Stage = stage1,
        BoardId = 1,
        Board = board1,
        Subparts = { subpart1, subpart2 },
        ResponsibleEmployeeId = 1
    };
    private readonly static Assignment assignment2 = new()
    {
        Id = 2,
        Topic = "Second Assignment",
        Description = "Other description",
        Deadline = new DateTime(2023, 04, 02),
        StageId = 2,
        Stage = stage2,
        BoardId = 1,
        Board = board1,
        Subparts = { subpart3, subpart4 },
        ResponsibleEmployeeId = 1
    };

    private readonly static Employee employee1 = new()
    {
        Id = 1,
        FirstName = "Test",
        LastName = "Employee",
        Boards =
        {
            board1,
            board2
        },
        Assignments =
        {
            assignment1,
            assignment2
        }
    };

    private readonly static User user1 = new()
    {
        Id = "12345678-1234-1234-1234-123456789012",
        UserName = "testUser",
        Email = "test@email.com",
        EmployeeId = 1,
        Employee = employee1
    };
}
