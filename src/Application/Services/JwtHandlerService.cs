using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class JwtHandlerService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public JwtHandlerService(IConfiguration configuration,
        UserManager<User> userManger)
    {
        _configuration = configuration;
        _userManager = userManger;
    }

    public async Task<JwtSecurityToken> GetTokenAsync(User user)
    {
        EnsureUserIsNotNull(user);

        var jwtOptions = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: await GetClaimsAsync(user),
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(
                _configuration["JwtSettings:ExpirationTimeInMinutes"])),
            signingCredentials: GetSigningCredentials());
        return jwtOptions;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(
            _configuration["JwtSettings:SecurityKey"]);
        var secret = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(secret,
            SecurityAlgorithms.HmacSha256);
        return signingCredentials;
    }

    private async Task<List<Claim>> GetClaimsAsync(User user)
    {
        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private void EnsureUserIsNotNull(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
    }

}
