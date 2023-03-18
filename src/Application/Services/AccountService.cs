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
        var user = await FindUserAsync(loginRequest.NameOrEmail);

        if (user is null || !await CheckPasswordAsync(user, loginRequest.Password))
            return GetLoginFailedResult();
        
        var token = await _jwtHandlerService.GetTokenAsync(user);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return GetLoginSucceedResult(jwt);
    }

    public async Task<RegistrationResponseModel> RegistrationAsync(
        RegistrationRequestModel registrationRequest)
    {
        var validationResult = ValidateRegistrationRequestModel(registrationRequest);
        if (!validationResult.Success)
            return validationResult;

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
            await _userManager.AddToRoleAsync(user, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
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

    public async Task<UserProfileModel?> GetUserProfileAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null) 
            return null;

        await LinkEmployeeToTheUser(user);

        return _mapper.Map<UserProfileModel>(user);
    }

    private async Task LinkEmployeeToTheUser(User user)
    {
        if (user.EmployeeId != null && user.Employee == null)
        {
            user.Employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == user.EmployeeId.Value);
        }
        else if ((await _userManager.GetRolesAsync(user)).Contains(DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE) &&
            user.Employee == null)
        {
            var employee = new Employee();
            await _context.Employees.AddAsync(employee);
            user.Employee = employee;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(string userName,
        UserProfileUpdateModel updatedUser)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) 
            return false;

        await UpdateUserInfo(user, updatedUser);
        try
        {
            await _userManager.UpdateAsync(user);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private async Task UpdateUserInfo(User user, UserProfileUpdateModel updatedUser)
    {
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
    }

    public async Task<bool> ChangePasswordAsync(string userName,
        ChangePasswordModel model)
    {
        User user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return false;
        }
        try
        {
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return result.Succeeded;
        }
        catch 
        { 
            return false; 
        }
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
        try
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        catch
        {
            return false;
        }
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

    private static RegistrationResponseModel ValidateRegistrationRequestModel(RegistrationRequestModel model)
    {
        RegistrationResponseModel response = new() { Success = true, Message = string.Empty };

        if (model.UserName == null)
        {
            response.Success = false;
            response.Message += "UserName is required\n";
        }
        else if (model.UserName.Length < 3)
        {
            response.Success = false;
            response.Message = "Username must be at least 3 characters long\n";
        }
        if (model.Email == null)
        {
            response.Success = false;
            response.Message += "Email is required\n";
        }
        if (model.Password == null)
        {
            response.Success = false;
            response.Message += "Password is required\n";
        }
        else if (model.Password.Length < 8)
        {
            response.Success = false;
            response.Message = "Password must be at least 8 characters long\n";
        }
        return response;
    }
}
