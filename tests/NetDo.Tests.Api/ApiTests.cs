namespace DigitalOcean.Tests.Api;

using DigitalOcean.Api;

public class ApiTests : TestsRuntime
{
    static ApiTests()
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
    
    protected static string apiKey;
    protected static string spacesEndpoint, spacesKeyId, spacesKeySecret, spacesBucket1;

    protected DigitalOceanClient client = new DigitalOceanClient(apiKey);
}
