namespace DigitalOcean.Tests.Api;

using DigitalOcean.Api;

public class ApiTests : TestsRuntime
{
    static ApiTests()
    {
        apiKey = GetRequiredValue(config, "ApiKey");
    }
    
    protected static string apiKey;

    protected DigitalOceanClient client = new DigitalOceanClient(apiKey);
}
