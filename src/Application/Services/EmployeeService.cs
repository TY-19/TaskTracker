﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly ITrackerDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    public EmployeeService(ITrackerDbContext context,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<EmployeeGetModel>> GetAllAsync()
    {
        IQueryable<Employee> employees = _context.Employees
                .Include(e => e.User)
                .AsNoTracking();
        return await GetEmployeeModelsWithRolesAsync(employees);
    }

    public async Task<IEnumerable<EmployeeGetModel>> GetAllEmployeeFromTheBoardAsync(int boardId)
    {
        IQueryable<Employee> employees = _context.Employees
                .Include(e => e.User)
                .Where(s => s.Boards.Select(b => b.Id).Contains(boardId))
                .AsNoTracking();
        return await GetEmployeeModelsWithRolesAsync(employees);
    }

    public async Task<EmployeeGetModel?> GetEmployeeByIdAsync(int id)
    {
        Employee? employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return null;

        return await GetEmployeeModelWithRolesAsync(employee);
    }

    private async Task<IEnumerable<EmployeeGetModel>> GetEmployeeModelsWithRolesAsync(
        IEnumerable<Employee> employees)
    {
        var models = new List<EmployeeGetModel>();
        foreach (var employee in employees)
            models.Add(await GetEmployeeModelWithRolesAsync(employee));

        return models;
    }
    private async Task<EmployeeGetModel> GetEmployeeModelWithRolesAsync(Employee employee)
    {
        var model = _mapper.Map<EmployeeGetModel>(employee);
        model.Roles = employee.User != null
            ? await _userManager.GetRolesAsync(employee.User)
            : new List<string>();
        return model;
    }

    public async Task AddEmployeeToTheBoardAsync(int boardId, string userNameOrId)
    {
        User user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userNameOrId || u.UserName == userNameOrId)
            ?? throw new ArgumentException("User with a such id or name does not exist", nameof(userNameOrId));

        user.Employee ??= new Employee();

        Board board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == boardId)
            ?? throw new ArgumentException("Board with a such id does not exist", nameof(boardId));

        board.Employees.Add(user.Employee);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmployeeFromTheBoardAsync(int boardId, int employeeId)
    {
        Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        Board? board = await _context.Boards
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(b => b.Id == boardId);

        if (employee == null || board == null)
            return;

        board.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
