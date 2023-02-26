using AutoMapper;
using Domain.Entities;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mapping;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
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
        CreateMap<Assignment, AssignmentPostPutModel>();
        CreateMap<Assignment, AssignmentGetModel>()
            .ForMember(bgm => bgm.Subparts, x => x.MapFrom(b => b.Subparts));
        
        CreateMap<AssignmentPostPutModel, Assignment>();

        CreateMap<Employee, EmployeeGetBoardModel>();

        CreateMap<WorkflowStagePostPutModel, WorkflowStage>();
        CreateMap<WorkflowStage, WorkflowStageGetModel>();
        CreateMap<Board, BoardGetModel>()
            .ForMember(bgm => bgm.Stages, x => x.MapFrom(b => b.Stages))
            .ForMember(bgm => bgm.Employees, x => x.MapFrom(b => b.Employees))
            .ForMember(bgm => bgm.Assignments, x => x.MapFrom(b => b.Assignments));


    }
}
