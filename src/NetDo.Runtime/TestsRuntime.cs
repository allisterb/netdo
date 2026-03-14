namespace DigitalOcean;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Extensions.Logging;

public class TestsRuntime : Runtime
{
    static TestsRuntime()
    {
        Runtime.WithFileAndConsoleLogging("DigitalOcean", "Tests", true);
        config = LoadConfigFile("testappsettings.json");      
        
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_API_TOKEN")))
        {
            Environment.SetEnvironmentVariable("DIGITALOCEAN_API_TOKEN", config["ApiKey"], EnvironmentVariableTarget.Process);
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN")))
        {
            Environment.SetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN", config["ApiKey2"], EnvironmentVariableTarget.Process);
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT")))
        {
            Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT", config["SpacesEndpoint"], EnvironmentVariableTarget.Process);
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_ID")))
        {
            Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_ID", config["SpacesAccessKeyId"], EnvironmentVariableTarget.Process);
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_SECRET")))
        {
            Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_SECRET", config["SpacesAccessKeySecret"], EnvironmentVariableTarget.Process);
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET")))
        {
            Environment.SetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET", config["SpacesSessionBucket"], EnvironmentVariableTarget.Process);
        }
    }    
    static protected IConfigurationRoot config;
}

