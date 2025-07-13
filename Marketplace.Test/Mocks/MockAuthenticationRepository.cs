using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Data.Repositories;
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
        {
            Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);
        }
        else
        {
            Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
        }

        if (updateAsyncFailed)
        {
            Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed());
        }
        else
        {
            Setup(x => x.UpdateUserAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
        }
    }
}
