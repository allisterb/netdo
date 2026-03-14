namespace DigitalOcean.Tests.Api;

using DigitalOcean.Api;
using System;

public class SpacesTests : ApiTests
{
    [Fact]
    public async Task CanListBuckets()
    {
        var space = new SpacesClient(s)
        var b = await client.Balance_getAsync();
        Assert.NotNull(b.Account_balance);
        Assert.NotNull(b.Generated_at);
    }
}
