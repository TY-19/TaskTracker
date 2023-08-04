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
    public void Mapper_MapsSubpartPostModelToSubpart()
    {
        IMapper mapper = GetMapper();
        SubpartPostModel subpartmodel = new()
        {
            Name = "Test",
            Description = "Description",
            PercentValue = 50
        };

        var result = mapper.Map<Subpart>(subpartmodel);

        Assert.Multiple(
            () => Assert.Equal(subpartmodel.Name, result.Name),
            () => Assert.Equal(subpartmodel.Description, result.Description),
            () => Assert.Equal(subpartmodel.PercentValue, result.PercentValue)
        );
    }
    [Fact]
    public void Mapper_MapsSubpartPutModelToSubpart()
    {
        IMapper mapper = GetMapper();
        SubpartPutModel subpartmodel = new()
        {
            Name = "Test",
            Description = "Description",
            PercentValue = 50
        };

        var result = mapper.Map<Subpart>(subpartmodel);

        Assert.Multiple(
            () => Assert.Equal(subpartmodel.Name, result.Name),
            () => Assert.Equal(subpartmodel.Description, result.Description),
            () => Assert.Equal(subpartmodel.PercentValue, result.PercentValue)
        );
    }
    [Fact]
    public void Mapper_MapsSubpartPutModelToSubpart_ChangesOnlyModifiedProperty()
    {
        IMapper mapper = GetMapper();
        Subpart subpart = new()
        {
            Name = "OldName",
            Description = "CorrectDescription",
            PercentValue = 25
        };

        SubpartPutModel subpartModel = new()
        {
            Name = "Test",
            PercentValue = 50
        };

        var result = mapper.Map(subpartModel, subpart);

        Assert.Multiple(
            () => Assert.Equal(subpartModel.Name, result.Name),
            () => Assert.Equal(subpart.Description, result.Description),
            () => Assert.Equal(subpartModel.PercentValue, result.PercentValue)
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
    public void Mapper_MapsAssignmentPostModelToAssignment()
    {
        IMapper mapper = GetMapper();
        AssignmentPostModel model = new()
        {
            Topic = "Topic 1",
            Description = "Description",
            Deadline = new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Local),
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
    public void Mapper_MapsAssignmentPutModelToAssignment()
    {
        IMapper mapper = GetMapper();
        AssignmentPutModel model = new()
        {
            Topic = "Topic 1",
            Description = "Description",
            Deadline = new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Local),
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
    public void Mapper_MapsAssignmentPutModelToAssignment_ChangesOnlyModifiedProperty()
    {
        IMapper mapper = GetMapper();
        Assignment assignment = new()
        {
            Topic = "Test Topic",
            Description = "Correct Description"
        };

        AssignmentPutModel assignmentModel = new()
        {
            Topic = "UpdatedTopic"
        };

        var result = mapper.Map(assignmentModel, assignment);

        Assert.Multiple(
            () => Assert.Equal(assignmentModel.Topic, result.Topic),
            () => Assert.Equal(assignment.Description, result.Description)
        );
    }

    [Fact]
    public void Mapper_MapsEmployeeToEmployeeGetBoardModel()
    {
        IMapper mapper = GetMapper();
        Employee employye = employee1;

        var result = mapper.Map<EmployeeGetModel>(employye);

        Assert.Multiple(
            () => Assert.Equal(employye.Id, result.Id),
            () => Assert.Equal(employye.FirstName, result.FirstName),
            () => Assert.Equal(employye.LastName, result.LastName)
        );
    }

    [Fact]
    public void Mapper_MapsEmployeePostModelToEmployee()
    {
        IMapper mapper = GetMapper();
        EmployeePostModel model = new()
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
    public void Mapper_MapsEmployeePutModelToEmployee()
    {
        IMapper mapper = GetMapper();
        EmployeePutModel model = new()
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
    public void Mapper_MapsEmployeePutModelToEmployee_ChangesOnlyModifiedProperty()
    {
        IMapper mapper = GetMapper();
        Employee employee = new()
        {
            FirstName = "First",
            LastName = "Last"
        };

        EmployeePutModel employeeModel = new()
        {
            FirstName = "Updated First"
        };

        var result = mapper.Map(employeeModel, employee);

        Assert.Multiple(
            () => Assert.Equal(employeeModel.FirstName, result.FirstName),
            () => Assert.Equal(employee.LastName, result.LastName)
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
    public void Mapper_MapsWorkflowStagePostModelToWorkflowStage()
    {
        IMapper mapper = GetMapper();
        WorkflowStagePostModel model = new()
        {
            Name = "Stage1"
        };

        var result = mapper.Map<WorkflowStage>(model);

        Assert.Equal(model.Name, result.Name);
    }
    [Fact]
    public void Mapper_MapsWorkflowStagePutModelToWorkflowStage()
    {
        IMapper mapper = GetMapper();
        WorkflowStagePutModel model = new()
        {
            Name = "Stage1"
        };

        var result = mapper.Map<WorkflowStage>(model);

        Assert.Equal(model.Name, result.Name);
    }

    [Fact]
    public void Mapper_MapsWorkflowStagePutToExistedWorkflowStage()
    {
        IMapper mapper = GetMapper();
        WorkflowStage stage = new()
        {
            Id = 1,
            Name = "Original",
            Position = 1,
        };
        WorkflowStagePutModel model = new()
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
    public void Mapper_MapsWorkflowStagePutModelToWorkflowStage_ChangesOnlyModifiedProperty()
    {
        IMapper mapper = GetMapper();
        WorkflowStage workflowStage = new()
        {
            Name = "Old name",
            Position = 1,
        };

        WorkflowStagePutModel workflowStageModel = new()
        {
            Name = "New name"
        };

        var result = mapper.Map(workflowStageModel, workflowStage);

        Assert.Multiple(
            () => Assert.Equal(workflowStageModel.Name, result.Name),
            () => Assert.Equal(workflowStage.Position, result.Position)
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
    public void Mapper_MapsBoardPostModelToBoard()
    {
        IMapper mapper = GetMapper();
        BoardPostModel model = new() { Name = "NewName" };

        var result = mapper.Map<Board>(model);

        Assert.Equal(model.Name, result.Name);
    }
    [Fact]
    public void Mapper_MapsBoardPutModelToBoard()
    {
        IMapper mapper = GetMapper();
        BoardPutModel model = new() { Name = "NewName" };

        var result = mapper.Map<Board>(model);

        Assert.Equal(model.Name, result.Name);
    }

    [Fact]
    public void Mapper_MapsBoardPutModelToExistedBoard()
    {
        IMapper mapper = GetMapper();
        Board board = new()
        {
            Id = 5,
            Name = "Board1",
        };
        BoardPutModel model = new() { Name = "NewName" };

        mapper.Map(model, board);

        Assert.Multiple(
            () => Assert.Equal(5, board.Id),
            () => Assert.Equal(model.Name, board.Name)
        );
    }
    [Fact]
    public void Mapper_MapsBoardPutModelToBoard_ChangesOnlyModifiedProperty()
    {
        IMapper mapper = GetMapper();
        Board board = new()
        {
            Id = 1,
            Name = "Old name"
        };

        BoardPutModel boardModel = new()
        {
            Name = "New name"
        };

        var result = mapper.Map(boardModel, board);

        Assert.Multiple(
            () => Assert.Equal(boardModel.Name, result.Name),
            () => Assert.Equal(board.Id, result.Id)
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
        Deadline = new DateTime(2023, 04, 01, 0, 0, 0, DateTimeKind.Local),
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
        Deadline = new DateTime(2023, 04, 02, 0, 0, 0, DateTimeKind.Local),
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
