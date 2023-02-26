using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
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
        var user = await FindUserAsync(loginRequest.Name);
        if (user is null || !await CheckPasswordAsync(user, loginRequest.Password))
        {
            return GetLoginFailedResult();
        }
        var token = await _jwtHandlerService.GetTokenAsync(user);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return GetLoginSucceedResult(jwt);
    }

    public async Task<RegistrationResponseModel> RegistrationAsync(
        RegistrationRequestModel registrationRequest)
    {
        if (await _userManager.FindByNameAsync(registrationRequest.UserName) != null)
        {
            return new RegistrationResponseModel()
            {
                Success = false,
                Message = "User with this name already exists"
            };
        }

        var user = new User()
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.Email,
        };
        try
        {
            await _userManager.CreateAsync(user, registrationRequest.Password);
            await _userManager.AddToRoleAsync(user, "Employee");
        }
        catch
        {
            return new RegistrationResponseModel()
            {
                Success = false,
                Message = "Name or password is not valid"
            };
        }
        return new RegistrationResponseModel()
        {
            Success = true,
            Message = "Registration was successfull"
        };
    }

    public async Task<UserProfileModel> GetUserProfile(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user.EmployeeId != null && user.Employee == null)
        {
            user.Employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == user.EmployeeId.Value);
        }
        return _mapper.Map<UserProfileModel>(user);
    }

    public async Task<bool> UpdateUserProfile(string userName,
        UserProfileUpdateModel updatedUser)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (updatedUser?.Email != null)
        {
            user.Email = updatedUser.Email;
        }
        if (user.Employee == null)
        {
            var employee = new Employee();
            await _context.Employees.AddAsync(employee);
            user.Employee = employee;
        }

        if (updatedUser?.FirstName != null)
        {
            user.Employee.FirstName = updatedUser.FirstName;
        }
        if (updatedUser?.LastName != null)
        {
            user.Employee.LastName = updatedUser.LastName;
        }
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(string userName,
        ChangePasswordModel model)
    {
        User user = await _userManager.FindByNameAsync(userName);
        if (user == null) 
        { 
            return false;
        }
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        return result.Succeeded;
    }

    private async Task<User?> FindUserAsync(string nameOrEmail)
    {
        User user;
        if (nameOrEmail.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(nameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(nameOrEmail);
        }
        return user;
    }

    private async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
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

    private static LoginResponseModel GetLoginSucceedResult(string? token)
    {
        if (string.IsNullOrEmpty(token))
            return GetLoginFailedResult();

        return new LoginResponseModel
        {
            Success = true,
            Message = "Login successful",
            Token = token
        };
    }
}
