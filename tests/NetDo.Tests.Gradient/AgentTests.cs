namespace DigitalOcean.Tests.Gradient;

using Microsoft.Extensions.AI;

using DigitalOcean.Gradient;

public class AgentTests : TestsRuntime
{
    [Fact]
    public async Task CanCreateAgent()
    {
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        Assert.NotNull(agent);       
        var r = await agent.GetResponseAsync("Can you create a booking for me?", new ChatOptions() { });


        Assert.NotNull(r);
    }

    public static string GetCurrentWeather(string city) => "Periods of rain or drizzle, 15 C";

    [Fact]
    public async Task CanCallTool()
    {
        AIFunction[] tools = [AIFunctionFactory.Create(GetCurrentWeather, name: "get_current_weather", description: "Get the current weather")];
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4", tools);
        var r = await agent.GetResponseAsync("Get the current weather in London.", options: new ChatOptions() { Tools = tools, ToolMode = ChatToolMode.Auto});
        Assert.NotNull(r);  
    }

    [Fact]
    public async Task CanQueryKB()
    {
        
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        var r = await agent.GetResponseAsync("List all the agents..");
        Assert.NotNull(r);
    }
}

