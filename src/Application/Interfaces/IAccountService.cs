using TaskTracker.Application.Models;

namespace TaskTracker.Application.Interfaces;

public interface IAccountService
{
    public Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequest);
    public Task<RegistrationResponseModel> RegistrationAsync(RegistrationRequestModel registrationRequest);
    public Task<UserProfileModel?> GetUserProfileAsync(string userName);
    public Task UpdateUserProfileAsync(string userName, UserProfileUpdateModel updatedUser);
    public Task ChangePasswordAsync(string userName, ChangePasswordModel model);
}
