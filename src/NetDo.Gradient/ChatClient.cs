namespace DigitalOcean.Gradient;

using System;

using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

using OpenAI;
using DigitalOcean.Api;

public class ChatClient : DelegatingChatClient
{
    #region Constructors
    private ChatClient(ApiAgent apiAgent, string? apikey=null) : base(GetOpenAIChatClient(apiAgent, apikey))
    {
        this.client = new DigitalOceanClient();
        this.apiAgent = apiAgent;        
    }

    public ChatClient(string uuid, string? apikey=null) : this(GetApiAgent(uuid), apikey) {}
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

    public static IChatClient GetOpenAIChatClient(ApiAgent agent, string? apikey=null)
    {
        var endpoint = agent.Deployment?.Url ?? throw new ArgumentNullException($"The agent {agent.Uuid} ({agent.Name}) deployment field or deployment url is null.");
        var _apikey = apikey ?? Environment.GetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN") ?? throw new ArgumentException("The apikey parameter is not set and the GRADIENT_AGENT_API_TOKEN environment variable is not set.");
       
        return 
            new OpenAIClient(new System.ClientModel.ApiKeyCredential(_apikey), new OpenAIClientOptions() 
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
            .UseChatReducer(new MessageCountingChatReducer(10))
            .Build();                
    }
    #endregion

    #region Fields
    internal DigitalOceanClient client;
    internal ApiAgent apiAgent;    
    #endregion
}
