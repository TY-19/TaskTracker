using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtHandlerService _jwtHandlerService;
    private readonly IMapper _mapper;
    private readonly ITrackerDbContext _context;
    public AccountService(UserManager<User> userManager,
        JwtHandlerService jwtHandlerService,
        IMapper mapper,
        ITrackerDbContext context)
    {
        _userManager = userManager;
        _jwtHandlerService = jwtHandlerService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequest)
    {
        User? user = await FindUserAsync(loginRequest.NameOrEmail);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            return GetLoginFailedResult();

        JwtSecurityToken token = await _jwtHandlerService.GetTokenAsync(user);
        string jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return await GetLoginSucceedResult(jwt, user);
    }

    private static LoginResponseModel GetLoginFailedResult()
    {
        return new LoginResponseModel
        {
            Success = false,
            Message = "Invalid Name/Email or Password",
            Token = null
        };
    }

    private async Task<LoginResponseModel> GetLoginSucceedResult(string token, User user)
    {
        return new LoginResponseModel
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserName = user.UserName,
            EmployeeId = user.EmployeeId,
            Roles = await _userManager.GetRolesAsync(user)
        };
    }

    public async Task<RegistrationResponseModel> RegistrationAsync(
        RegistrationRequestModel registrationRequest)
    {
        if (await _userManager.FindByNameAsync(registrationRequest.UserName) != null)
            return GetRegistrationResult(false, "User with this name already exists");

        User user = GetUserFromRegistrationRequest(registrationRequest);
        try
        {
            await _userManager.CreateAsync(user, registrationRequest.Password);
            await _userManager.AddToRoleAsync(user, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
        }
        catch
        {
            return GetRegistrationResult(false, "Name or password is not valid");
        }
        return GetRegistrationResult(true, "Registration was successfull");
    }

    private static RegistrationResponseModel GetRegistrationResult(bool success, string message)
    {
        return new RegistrationResponseModel()
        {
            Success = success,
            Message = message
        };
    }

    private static User GetUserFromRegistrationRequest(
        RegistrationRequestModel registrationRequest)
    {
        return new User()
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.Email,
            Employee = new Employee()
        };
    }

    public async Task<UserProfileModel?> GetUserProfileAsync(string userName)
    {
        User? user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.UserName == userName);

        return _mapper.Map<UserProfileModel>(user);
    }

    public async Task UpdateUserProfileAsync(string userName,
        UserProfileUpdateModel updatedUser)
    {
        User user = await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.UserName == userName)
            ?? throw new ArgumentException("User does not exist");

        await UpdateUserInfo(user, updatedUser);
    }

    private async Task UpdateUserInfo(User user, UserProfileUpdateModel updatedUser)
    {
        if (updatedUser == null)
            return;

        user.Email = updatedUser.Email ?? user.Email;
        user.Employee ??= await CreateNewEmployeeAsync();
        user.Employee.FirstName = updatedUser.FirstName ?? user.Employee.FirstName;
        user.Employee.LastName = updatedUser.LastName ?? user.Employee.LastName;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    private async Task<Employee> CreateNewEmployeeAsync()
    {
        var employee = new Employee();
        await _context.Employees.AddAsync(employee);
        return employee;
    }

    public async Task ChangePasswordAsync(string userName,
        ChangePasswordModel model)
    {
        User user = await _userManager.FindByNameAsync(userName)
            ?? throw new ArgumentException("User does not exist");

        IdentityResult result;
        try
        {
            result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        }
        catch
        {
            throw new ArgumentException("Password has not been changed");
        }
        if (!result.Succeeded)
            throw new ArgumentException("Password has not been changed");
    }

    private async Task<User?> FindUserAsync(string nameOrEmail)
    {
        return nameOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(nameOrEmail)
            : await _userManager.FindByNameAsync(nameOrEmail);
    }
}
