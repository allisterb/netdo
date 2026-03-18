namespace DigitalOcean.Tests.Gradient;

using DigitalOcean.Api;
using DigitalOcean.Gradient;
using System;
using System.Threading.Tasks;
using Xunit;

public class DurableSessionTests : TestsRuntime
{
    static DurableSessionTests()
    {
        apiKey = GetRequiredValue(config, "ApiKey");
        spacesEndpoint = GetRequiredValue(config, "SpacesEndpoint");
        spacesKeyId = GetRequiredValue(config, "SpacesKeyId");
        spacesKeySecret = GetRequiredValue(config, "SpacesKeySecret");
        spacesBucket1 = GetRequiredValue(config, "SpacesKeySecret");

        Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT", spacesEndpoint, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_ID", spacesKeyId, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_SECRET", spacesKeySecret, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET", spacesBucket1, EnvironmentVariableTarget.Process);
    }

    [Fact]
    public async Task CanSerializeAndDeserializeSession()
    {
        // Skip if environment variables are not set
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_ID")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_SECRET")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET")))
        {
            return;
        }

        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        var session = await agent.CreateSessionAsync();

        // 1. Serialize using public method
        var serializedState = await agent.SerializeSessionAsync(session);
        var id = session.StateBag.GetValue<string>("Id");
        Assert.Equal(id, serializedState.GetProperty("Id").GetString());
        Assert.Equal(1, serializedState.GetProperty("VersionId").GetInt64());
        

        // 2. Deserialize using public method
        var deserializedSession = await agent.DeserializeSessionAsync(serializedState);
        Assert.NotNull(deserializedSession);
        Assert.Equal(id, session.StateBag.GetValue<string>("Id"));
        
    }

    protected static string apiKey;
    protected static string spacesEndpoint, spacesKeyId, spacesKeySecret, spacesBucket1;

    protected DigitalOceanClient client = new DigitalOceanClient(apiKey);
   
}
