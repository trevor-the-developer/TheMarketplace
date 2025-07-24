using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Data.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindUserByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        var role = new IdentityRole(roleName);
        return await _roleManager.CreateAsync(role);
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        return await _userManager.GetClaimsAsync(user);
    }
}