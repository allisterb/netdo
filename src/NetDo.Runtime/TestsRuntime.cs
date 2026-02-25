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
    }    
    static protected IConfigurationRoot config;
}

