using Xunit;

namespace Marketplace.Test.Infrastructure;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}