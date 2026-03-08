namespace NetDo.Tests.Gradient;

using DigitalOcean;
using DigitalOcean.Gradient;

public class AgentTests : TestsRuntime
{
    [Fact]
    public async Task CanCreateAgent()
    {
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        Assert.NotNull(agent);
        var r = await agent.PromptAsync("Can you create a booking for me?");
        Assert.NotNull(r);
    }
}
