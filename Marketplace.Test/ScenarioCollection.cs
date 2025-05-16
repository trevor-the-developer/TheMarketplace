using Alba;
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
    public IAlbaHost? AlbaHost;

    public async Task InitializeAsync()
    {
        // This is absolutely necessary if you 
        // use Oakton for command line processing and want to 
        // use WebApplicationFactory and/or Alba for integration testing
        OaktonEnvironment.AutoStartHost = true;

        // var jwtSecurityStub = new JwtSecurityStub()
        //     .With("foo", "bar")
        //     .With(JwtRegisteredClaimNames.Email, "guy@company.com");

        // AlbaHost = await Alba.AlbaHost.For<global::Program>();
        AlbaHost = await Alba.AlbaHost.For<Program>();
        // AlbaHost = await Alba.AlbaHost.For<Program>(builder =>
        // {
        //     // Configure services etc
        // });
        //AlbaHost = await Alba.AlbaHost.For<Program>(jwtSecurityStub);
    }

    public async Task DisposeAsync()
    {
        if (AlbaHost != null) await AlbaHost.DisposeAsync();
    }
}

[Collection("scenarios")]
public abstract class ScenarioContext(WebAppFixture fixture)
{
    protected IAlbaHost Host { get; } = fixture.AlbaHost!;
}