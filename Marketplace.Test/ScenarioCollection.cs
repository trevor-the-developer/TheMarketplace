using System.Threading.Tasks;
using Alba;
using Marketplace.Test.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Oakton;
using Xunit;

namespace Marketplace.Test;

[CollectionDefinition("scenarios")]
public class ScenarioCollection : ICollectionFixture<WebAppFixture>
{
    //
}

public class WebAppFixture : IAsyncLifetime
{
    private readonly DatabaseTestFixture _databaseFixture;
    public IAlbaHost? AlbaHost;

    public WebAppFixture()
    {
        _databaseFixture = new DatabaseTestFixture();
    }

    public async Task InitializeAsync()
    {
        // Initialize database first
        await _databaseFixture.InitializeAsync();

        // Add a small delay to ensure database seeding is completely finished
        // before starting the Alba host
        await Task.Delay(1000);

        // This is absolutely necessary if you 
        // use Oakton for command line processing and want to 
        // use WebApplicationFactory and/or Alba for integration testing
        OaktonEnvironment.AutoStartHost = true;

        // var jwtSecurityStub = new JwtSecurityStub()
        //     .With("foo", "bar")
        //     .With(JwtRegisteredClaimNames.Email, "guy@company.com");

        // AlbaHost = await Alba.AlbaHost.For<global::Program>();
        AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        {
            // Set the environment to Development to use the real database
            builder.UseEnvironment("Development");
        });
        // AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        // {
        //     // Configure services etc
        // });
        //AlbaHost = await Alba.AlbaHost.For<Program>(jwtSecurityStub);
    }

    public async Task DisposeAsync()
    {
        if (AlbaHost != null) await AlbaHost.DisposeAsync();
        await _databaseFixture.DisposeAsync();
    }
}

[Collection("scenarios")]
public abstract class ScenarioContext(WebAppFixture fixture)
{
    protected IAlbaHost Host { get; } = fixture.AlbaHost!;
}