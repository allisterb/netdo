namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DigitalOcean.Api;

public class DonnaApi
{
    public ApiAgentPublic[]? ListAgents() => client.Genai_list_agentsAsync(null, null, null).GetAwaiter().GetResult()?.Agents?.ToArray();

    public ApiAgent? GetAgent(string uuid) => client.Genai_get_agentAsync(uuid).GetAwaiter().GetResult()?.Agent;


    protected DigitalOceanClient client = new DigitalOceanClient();
}
