using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Data.Interfaces;
using Marketplace.Test.Data;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Marketplace.Test.Mocks;

public class MockAuthenticationRepository : Mock<IAuthenticationRepository>
{
    public MockAuthenticationRepository(
        ApplicationUser? user = null,
        bool nullEmail = false,
        bool invalidPassword = false,
        bool updateAsyncFailed = false)
    {
        // Setup default responses
        Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        Setup(x => x.CreateRoleAsync(It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("test-email-confirmation-token");

        Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        Setup(x => x.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        Setup(x => x.DeleteUserAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        if (user != null)
        {
            Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Role.User.ToString() });

            Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim> { new(JwtRegisteredClaimNames.Name, user.Email ?? string.Empty) });

            Setup(x => x.FindUserByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        if (nullEmail)
        {
            Setup(x => x.FindUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);
        }
        else
        {
            // Only return user for specific email addresses that should exist
            Setup(x => x.FindUserByEmailAsync(It.Is<string>(e => e != TestData.TestUserOne)))
                .ReturnsAsync(user);

            // Return null for the test email
            Setup(x => x.FindUserByEmailAsync(TestData.TestUserOne))
                .ReturnsAsync((ApplicationUser?)null);
        }

        if (invalidPassword)
            Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);
        else
            Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);

        if (updateAsyncFailed)
            Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed());
        else
            Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
    }

    public void SetupCreateUserFailed(List<IdentityError> errors)
    {
        Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));
    }

    public void SetupCreateRoleFailed()
    {
        Setup(x => x.CreateRoleAsync(It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
    }

    public void SetupAddToRoleFailed()
    {
        Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
    }

    public void SetupGenerateEmailTokenFailed()
    {
        Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ThrowsAsync(new InvalidOperationException("Failed to generate email confirmation token"));
    }

    public void SetupUserNotFound()
    {
        Setup(x => x.FindUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
    }

    public void SetupConfirmEmailFailed()
    {
        Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
    }
}