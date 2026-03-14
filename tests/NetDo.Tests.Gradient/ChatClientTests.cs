namespace DigitalOcean.Tests.Gradient;

using Microsoft.Extensions.AI;

using DigitalOcean.Gradient;

public class ChatClientTests : TestsRuntime
{
    [Fact]
    public async Task CanCreateChatClient()
    {
        var agent = new ChatClient("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        Assert.NotNull(agent);       
        var r = await agent.GetResponseAsync("Can you create a booking for me?");

        Assert.NotNull(r);
    }
    [Fact]
    public async Task CanQueryKB()
    {
        
        var agent = new ChatClient("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        var r = await agent.GetResponseAsync("List all the agents..");
        Assert.NotNull(r);
    }
}

