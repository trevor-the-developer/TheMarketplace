using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Test.Mocks;

/// <summary>
/// Enhanced MockRoleManager with comprehensive options for registration testing
/// </summary>
public class EnhancedMockRoleManager : Mock<RoleManager<IdentityRole>>
{
    private readonly Mock<RoleManager<IdentityRole>> _roleManager;

    public EnhancedMockRoleManager(IdentityRole? role = null, MockRoleManagerOptions? options = null)
    {
        options ??= new MockRoleManagerOptions();

        _roleManager = new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

        SetupCreateAsync(options);
        SetupUpdateAsync(options);
        SetupDeleteAsync(options);
        SetupRoleExistsAsync(role, options);
        SetupFindByNameAsync(role, options);
        SetupFindByIdAsync(role, options);
    }

    private void SetupCreateAsync(MockRoleManagerOptions options)
    {
        var result = options.CreateAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "RoleCreateFailed", Description = "Failed to create role" }
            })
            : IdentityResult.Success;

        _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(result);
    }

    private void SetupUpdateAsync(MockRoleManagerOptions options)
    {
        var result = options.UpdateAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "RoleUpdateFailed", Description = "Failed to update role" }
            })
            : IdentityResult.Success;

        _roleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(result);
    }

    private void SetupDeleteAsync(MockRoleManagerOptions options)
    {
        var result = options.DeleteAsyncFailed
            ? IdentityResult.Failed(options.CustomErrors?.ToArray() ?? new[]
            {
                new IdentityError { Code = "RoleDeleteFailed", Description = "Failed to delete role" }
            })
            : IdentityResult.Success;

        _roleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(result);
    }

    private void SetupRoleExistsAsync(IdentityRole? role, MockRoleManagerOptions options)
    {
        if (options.RoleDoesNotExist || role == null)
        {
            _roleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
        }
        else
        {
            _roleManager.Setup(x => x.RoleExistsAsync(role.Name!))
                .ReturnsAsync(true);
        }
    }

    private void SetupFindByNameAsync(IdentityRole? role, MockRoleManagerOptions options)
    {
        if (options.RoleDoesNotExist || role == null)
        {
            _roleManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityRole?)null);
        }
        else
        {
            _roleManager.Setup(x => x.FindByNameAsync(role.Name!))
                .ReturnsAsync(role);
        }
    }

    private void SetupFindByIdAsync(IdentityRole? role, MockRoleManagerOptions options)
    {
        if (options.RoleDoesNotExist || role == null)
        {
            _roleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityRole?)null);
        }
        else
        {
            _roleManager.Setup(x => x.FindByIdAsync(role.Id))
                .ReturnsAsync(role);
        }
    }

    public override RoleManager<IdentityRole> Object => _roleManager.Object;
}
