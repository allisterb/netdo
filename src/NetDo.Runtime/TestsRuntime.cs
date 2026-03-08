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
    }    
    static protected IConfigurationRoot config;
}

