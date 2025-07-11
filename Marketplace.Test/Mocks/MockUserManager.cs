using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Test.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Marketplace.Test.Mocks;

public class MockUserManager : Mock<UserManager<ApplicationUser>>
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    public MockUserManager(
        ApplicationUser user = null!,
        bool nullEmail = false,
        bool invalidPassword = false,
        bool updateAsyncFailed = false)
    {
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

        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

        if (user != null!)
        {
            _userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Role.User.ToString() });
            _userManager.Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim> { new(JwtRegisteredClaimNames.Name, user.Email ?? string.Empty) });
            _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        if (nullEmail)
        {
            _userManager.Setup(x => x.FindByEmailAsync(null!))
                .ReturnsAsync((ApplicationUser)null!);
        }
        else
        {
            // Only return user for specific email addresses that should exist
            _userManager.Setup(x => x.FindByEmailAsync(It.Is<string>(e => e != TestData.TestUserOne)))
                .ReturnsAsync(user);

            // Return null for the test email
            _userManager.Setup(x => x.FindByEmailAsync(TestData.TestUserOne))
                .ReturnsAsync((ApplicationUser)null!);
        }


        if (invalidPassword)
            _userManager.Setup(x =>
                x.CheckPasswordAsync(user!, It.IsAny<string>())).ReturnsAsync(false);
        else
            _userManager.Setup(x =>
                x.CheckPasswordAsync(user!, It.IsAny<string>())).ReturnsAsync(true);

        if (updateAsyncFailed)
            _userManager.Setup(x =>
                x.UpdateAsync(user!)).ReturnsAsync(IdentityResult.Failed());
        else
            _userManager.Setup(x =>
                x.UpdateAsync(user!)).ReturnsAsync(IdentityResult.Success);

        _userManager.Setup(x => x.FindByEmailAsync(TestData.TestEmail));
    }

    public override UserManager<ApplicationUser> Object => _userManager.Object;
}