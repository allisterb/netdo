namespace DigitalOcean.Tests.Gradient;

using System;
using System.Collections.Generic;
using System.Text;

using DigitalOcean.Gradient;

public class AgentTests : TestsRuntime
{
    [Fact]
    public async Task CanCreateAgent()
    {
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");        
        Assert.NotNull(agent.Id);
        var r = await agent.RunAsync("List all the projects;");
        Assert.NotNull(r);   
    }
}
