using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Marketplace.Test.Mocks;

public class MockDbContext : Mock<MarketplaceDbContext>
{
    public MockDbContext()
        : base(new DbContextOptions<MarketplaceDbContext>())
    {
        // You can set up your DbSet mocks here
        // Example:
        // var mockUsers = new Mock<DbSet<ApplicationUser>>();
        // Setup(x => x.Users).Returns(mockUsers.Object);
    }
}