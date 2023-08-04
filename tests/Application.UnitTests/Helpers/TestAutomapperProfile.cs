using AutoMapper;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.UnitTests.Helpers;

internal class TestAutomapperProfile : Profile
{
    public TestAutomapperProfile()
    {
        CreateMap<User, UserProfileModel>()
            .ForMember(upm => upm.Id, x => x.MapFrom(u => u.Id))
            .ForMember(upm => upm.UserName, x => x.MapFrom(u => u.UserName))
            .ForMember(upm => upm.Email, x => x.MapFrom(u => u.Email))
            .ForMember(upm => upm.EmployeeId, x => x.MapFrom(u => u.EmployeeId))
            .ForMember(upm => upm.FirstName, x => x.MapFrom((src, dest) => src.Employee?.FirstName))
            .ForMember(upm => upm.LastName, x => x.MapFrom((src, dest) => src.Employee?.LastName))
            .ForMember(upm => upm.BoardsIds, x =>
                x.MapFrom((src, dest) => src.Employee?.Boards.Select(b => b.Id)))
            .ForMember(upm => upm.AssignmentsIds, x =>
                x.MapFrom((src, dest) => src.Employee?.Assignments.Select(b => b.Id)));

        CreateMap<Subpart, SubpartGetModel>();
        CreateMap<SubpartPostModel, Subpart>();
        CreateMap<SubpartPutModel, Subpart>();

        CreateMap<Assignment, AssignmentGetModel>()
            .ForMember(bgm => bgm.Subparts, x => x.MapFrom(b => b.Subparts));
        CreateMap<AssignmentPostModel, Assignment>()
            .ForMember(am => am.StageId, x => x.MapFrom((src, dest) => src.StageId))
            .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember != null));
        CreateMap<AssignmentPutModel, Assignment>()
            .ForMember(am => am.StageId, x => x.MapFrom((src, dest) => src.StageId ?? dest.StageId))
            .ForAllMembers(opt => opt.Condition((_, _, sourceMember) => sourceMember != null));

        CreateMap<Employee, EmployeeGetModel>();
        CreateMap<EmployeePostModel, Employee>();
        CreateMap<EmployeePutModel, Employee>();

        CreateMap<WorkflowStage, WorkflowStageGetModel>();
        CreateMap<WorkflowStagePostModel, WorkflowStage>();
        CreateMap<WorkflowStagePutModel, WorkflowStage>();

        CreateMap<Board, BoardGetModel>()
            .ForMember(bgm => bgm.Stages, x => x.MapFrom(b => b.Stages))
            .ForMember(bgm => bgm.Employees, x => x.MapFrom(b => b.Employees))
            .ForMember(bgm => bgm.Assignments, x => x.MapFrom(b => b.Assignments));
        CreateMap<BoardPostModel, Board>();
        CreateMap<BoardPutModel, Board>();
    }
}
