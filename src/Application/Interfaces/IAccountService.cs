using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IAccountService
{
    public Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequest);
    public Task<RegistrationResponseModel> RegistrationAsync(RegistrationRequestModel registrationRequest);
    public Task<UserProfileModel?> GetUserProfileAsync(string userName);
    public Task<bool> UpdateUserProfileAsync(string userName, UserProfileUpdateModel updatedUser);
    public Task<bool> ChangePasswordAsync(string userName, ChangePasswordModel model);
}
