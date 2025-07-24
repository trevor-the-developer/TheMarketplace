using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Test.Mocks;

public class MockRoleManager : Mock<RoleManager<IdentityRole>>
{
    private readonly Mock<RoleManager<IdentityRole>> _roleManager;

    public MockRoleManager(IdentityRole role = null!)
    {
        _roleManager = new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

        _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);

        if (role != null!)
        {
            _roleManager.Setup(x => x.FindByIdAsync(role.Id))
                .ReturnsAsync(role);
            _roleManager.Setup(x => x.FindByNameAsync(role.Name!))
                .ReturnsAsync(role);
            _roleManager.Setup(x => x.RoleExistsAsync(role.Name!))
                .ReturnsAsync(true);
        }

        // Add common role operations
        _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        _roleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
    }

    public override RoleManager<IdentityRole> Object => _roleManager.Object;
}