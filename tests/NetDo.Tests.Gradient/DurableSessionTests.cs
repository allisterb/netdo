namespace DigitalOcean.Tests.Gradient;

using System.Threading.Tasks;
using Xunit;

using DigitalOcean.Gradient;

public class DurableSessionTests : TestsRuntime
{
    [Fact]
    public async Task CanSerializeAndDeserializeSession()
    {
        // Skip if environment variables are not set
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_ID")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_SECRET")) ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET")))
        {
            return;
        }

        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        var sessionId = Guid.NewGuid().ToString();
        var session = new DurableAgentSession ();

        // 1. Serialize using public method
        var serializedState = await agent.SerializeSessionAsync(session);
        Assert.Equal(sessionId, serializedState.GetProperty("Id").GetString());
        Assert.Equal(1, serializedState.GetProperty("VersionId").GetInt64());
        Assert.NotNull(serializedState.GetProperty("S3VersionId").GetString());

        // 2. Deserialize using public method
        var deserializedSession = await agent.DeserializeSessionAsync(serializedState) as DurableAgentSession;
        Assert.NotNull(deserializedSession);
        Assert.Equal(sessionId, deserializedSession.Id);
        Assert.Equal(1, deserializedSession.VersionId);
    }
}
