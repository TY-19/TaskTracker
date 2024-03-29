﻿namespace TaskTracker.Application.Models;

public class BoardGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<WorkflowStageGetModel> Stages { get; set; } = new List<WorkflowStageGetModel>();
    public ICollection<EmployeeGetModel> Employees { get; set; } = new List<EmployeeGetModel>();
    public ICollection<AssignmentGetModel> Assignments { get; set; } = new List<AssignmentGetModel>();
}
