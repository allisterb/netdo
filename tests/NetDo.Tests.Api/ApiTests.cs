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
    }
    
    protected static string apiKey;
    protected static string spacesEndpoint, spacesKeyId, spacesKeySecret;

    protected DigitalOceanClient client = new DigitalOceanClient(apiKey);
}
