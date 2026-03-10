namespace DigitalOcean.Gradient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;
using OpenAI;
using DigitalOcean.Api;

public class Agent : DelegatingChatClient
{
    #region Constructors
    private Agent(ApiAgent apiAgent, params AIFunction[] tools) : base(GetOpenAIChatClient(apiAgent, tools))
    {
        this.apiAgent = apiAgent;
        this.client = new DigitalOceanClient();        
    }

    public Agent(string uuid, params AIFunction[] tools) : this(GetApiAgent(uuid), tools) {}
    #endregion

    #region Properties
    public string Uuid => apiAgent.Uuid!;
    #endregion

    #region Methods
    public static ApiAgent GetApiAgent(string uuid)
    {
        var client = new DigitalOceanClient();
        try
        {
            var response = client.Genai_get_agentAsync(uuid).GetAwaiter().GetResult();
            if (response is not null && response.Agent is not null)
            {
                return response.Agent;
            }
            else
            {
                throw new Exception($"An unknown error occurred attempting to retrieve agent with uuid {uuid}.");
            }
        }
        catch (DigitalOceanApiException<Error> e)
        {
            throw new Exception($"An error occurred attempting to retrieve agent with uuid {uuid}: {e.Result.Message}.", e);
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred attempting to retrieve agent with uuid {uuid}.", e);
        }
    }

    public static IChatClient GetOpenAIChatClient(ApiAgent agent, params AIFunction[] tools)
    {
        var endpoint = agent.Deployment?.Url ?? throw new ArgumentNullException($"The agent {agent.Uuid} ({agent.Name}) deployment field or deployment url is null.");
        var apikey = Environment.GetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN") ?? throw new ArgumentNullException("The GRADIENT_AGENT_API_TOKEN environment variable is not set.");
        
        return 
            new OpenAIClient(new System.ClientModel.ApiKeyCredential(apikey), new OpenAIClientOptions() 
            { 
                Endpoint = new Uri(endpoint + "/api/v1"),
                ClientLoggingOptions = new System.ClientModel.Primitives.ClientLoggingOptions() 
                {
                    EnableLogging = true,
                    LoggerFactory = Runtime.loggerFactory,     
                    EnableMessageContentLogging = true,
                    EnableMessageLogging = true
                },                        
            })
            .GetChatClient(agent.Model!.Inference_name)            
            .AsIChatClient()
            .AsBuilder()            
            .UseLogging(Runtime.loggerFactory)           
            .UseFunctionInvocation(Runtime.loggerFactory, f =>
            {
                f.AdditionalTools = tools;               
                f.TerminateOnUnknownCalls = false;   
            })            
            .Build();                
    }
    #endregion

    #region Fields
    protected ApiAgent apiAgent;
    protected DigitalOceanClient client;    
    #endregion
}
