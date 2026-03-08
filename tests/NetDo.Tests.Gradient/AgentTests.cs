namespace NetDo.Tests.Gradient;

using DigitalOcean;
using DigitalOcean.Gradient;

public class AgentTests : TestsRuntime
{
    [Fact]
    public void CanCreateAgent()
    {
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        Assert.NotNull(agent);
    }
}
