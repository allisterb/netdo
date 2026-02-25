namespace DigitalOcean.Tests.Api;

using System;

public class AccountTests : ApiTests
{
    [Fact]
    public async Task CanGetBalance()
    {
        var b = await client.Balance_getAsync();
        Assert.NotNull(b.Account_balance);
        Assert.NotNull(b.Generated_at);
    }
}
