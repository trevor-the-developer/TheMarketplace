using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Marketplace.Data.Repositories;

public interface IAuthenticationRepository
{
    Task<ApplicationUser?> FindUserByEmailAsync(string email);
    Task<ApplicationUser?> FindUserByNameAsync(string userName);
    Task<ApplicationUser?> FindUserByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
    Task<IdentityResult> CreateRoleAsync(string roleName);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
}
