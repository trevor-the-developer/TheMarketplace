using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Marketplace.Test.Mocks;

/// <summary>
/// Enhanced MockUserManager with comprehensive options for registration testing
/// </summary>
public class EnhancedMockUserManager : Mock<UserManager<ApplicationUser>>
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    public EnhancedMockUserManager(ApplicationUser? user = null, MockUserManagerOptions? options = null)
    {
        options ??= new MockUserManagerOptions();

        _userManager = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        SetupFindByEmailAsync(user, options);
        SetupFindByIdAsync(user, options);
        SetupCreateAsync(options);
        SetupUpdateAsync(options);
        SetupDeleteAsync(options);
        SetupCheckPasswordAsync(options);
        SetupAddToRoleAsync(options);
        SetupGenerateEmailConfirmationTokenAsync(options);
        SetupConfirmEmailAsync(options);
        SetupGetRolesAsync(user);
        SetupGetClaimsAsync(user);
        SetupFindByNameAsync(user);
    }

    private void SetupFindByEmailAsync(ApplicationUser? user, MockUserManagerOptions options)
    {
        if (options.UserNotFound || user == null)
        {
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
        }
        else
        {
            _userManager.Setup(x => x.FindByEmailAsync(user.Email!))
                .ReturnsAsync(user);
        }
    }

    private void SetupFindByIdAsync(ApplicationUser? user, MockUserManagerOptions options)
    {
        if (options.UserNotFound || user == null)
        {
            _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
        }
        else
        {
            _userManager.Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);
        }
    }

    private void SetupCreateAsync(MockUserManagerOptions options)
    {
        var result = options.CreateAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "CreateFailed", Description = "Failed to create user" }
            })
            : IdentityResult.Success;

        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    private void SetupUpdateAsync(MockUserManagerOptions options)
    {
        var result = options.UpdateAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "UpdateFailed", Description = "Failed to update user" }
            })
            : IdentityResult.Success;

        _userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(result);
    }

    private void SetupDeleteAsync(MockUserManagerOptions options)
    {
        var result = options.DeleteAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "DeleteFailed", Description = "Failed to delete user" }
            })
            : IdentityResult.Success;

        _userManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(result);
    }

    private void SetupCheckPasswordAsync(MockUserManagerOptions options)
    {
        _userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(!options.InvalidPassword);
    }

    private void SetupAddToRoleAsync(MockUserManagerOptions options)
    {
        var result = options.AddToRoleAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "AddToRoleFailed", Description = "Failed to add user to role" }
            })
            : IdentityResult.Success;

        _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    private void SetupGenerateEmailConfirmationTokenAsync(MockUserManagerOptions options)
    {
        if (options.GenerateEmailConfirmationTokenAsyncFailed)
        {
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ThrowsAsync(new InvalidOperationException("Failed to generate email confirmation token"));
        }
        else
        {
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(options.EmailConfirmationToken ?? "test-confirmation-token");
        }
    }

    private void SetupConfirmEmailAsync(MockUserManagerOptions options)
    {
        var result = options.ConfirmEmailAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "ConfirmEmailFailed", Description = "Failed to confirm email" }
            })
            : IdentityResult.Success;

        _userManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    private void SetupGetRolesAsync(ApplicationUser? user)
    {
        if (user != null)
        {
            _userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Role.User.ToString() });
        }
    }

    private void SetupGetClaimsAsync(ApplicationUser? user)
    {
        if (user != null)
        {
            _userManager.Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim> { new(JwtRegisteredClaimNames.Name, user.Email ?? string.Empty) });
        }
    }

    private void SetupFindByNameAsync(ApplicationUser? user)
    {
        if (user != null)
        {
            _userManager.Setup(x => x.FindByNameAsync(user.UserName!))
                .ReturnsAsync(user);
        }
    }

    public override UserManager<ApplicationUser> Object => _userManager.Object;
}
