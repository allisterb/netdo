namespace DigitalOcean.Gradient;

using System;
using System.Collections.Generic;
using System.Text.Json;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using DigitalOcean.Api;

public class Agent : AIAgent
{
    #region Constructors   
    public Agent(string uuid)
    {
        this.chatClient = new ChatClient(uuid);
        this.client = this.chatClient.client;        
        this.apiAgent = this.chatClient.apiAgent;
        this.aiAgent = this.chatClient.AsAIAgent(instructions: this.apiAgent.Instruction, name: this.apiAgent.Name, description: this.apiAgent.Description, loggerFactory: Runtime.loggerFactory);
    }
    #endregion

    #region Properties
    protected override string? IdCore => this.chatClient.Uuid;
    #endregion

    #region Methods
    protected override ValueTask<AgentSession> CreateSessionCoreAsync(CancellationToken cancellationToken) => this.aiAgent.CreateSessionAsync(cancellationToken); //ValueTask.FromResult<AgentSession>(new DurableAgentSession());
        
    protected override Task<AgentResponse> RunCoreAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)    
        => this.aiAgent.RunAsync(messages, session, options, cancellationToken);
    
    protected override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
        => this.aiAgent.RunStreamingAsync(messages, session, options, cancellationToken);

    protected override ValueTask<JsonElement> SerializeSessionCoreAsync(AgentSession session, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        if (session is not DurableAgentSession durableSession)
        {
            throw new ArgumentException("Session must be of type DurableAgentSession.");
        }       
        durableSession.VersionId++;
        var json = JsonSerializer.SerializeToUtf8Bytes(durableSession, jsonSerializerOptions);        
        return ValueTask.FromResult(JsonSerializer.SerializeToElement(durableSession, jsonSerializerOptions));
    }

    protected override async ValueTask<AgentSession> DeserializeSessionCoreAsync(JsonElement serializedState, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        return serializedState.Deserialize<DurableAgentSession>(jsonSerializerOptions) ?? throw new Exception("Could not deserialize session");        
    }


    protected async Task SaveSessionAsync(AgentSession session, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        if (session is not DurableAgentSession durableSession)
        {
            throw new ArgumentException("Session must be of type DurableAgentSession.");
        }
        EnsureSpacesClient();
        durableSession.VersionId++;
        var json = JsonSerializer.SerializeToUtf8Bytes(durableSession, jsonSerializerOptions);
        await this.spacesClient!.PutObjectAsync(this.sessionBucket!, durableSession.Id, json);                
    }

    protected async ValueTask<AgentSession> RestoreSessionAsync(string sessionid, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        EnsureSpacesClient();
        var json = await this.spacesClient!.GetObjectAsync(this.sessionBucket!, sessionid);
        var durableSession = JsonSerializer.Deserialize<DurableAgentSession>(json, jsonSerializerOptions);
        return durableSession ?? throw new InvalidOperationException("Failed to deserialize session.");
    }

    protected void EnsureSpacesClient()
    {
        if (this.spacesClient != null) return;

        var endpoint = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT") ?? throw new InvalidOperationException("DIGITALOCEAN_SPACES_ENDPOINT not set.");
        var accessKey = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_ID") ?? throw new InvalidOperationException("DIGITALOCEAN_SPACES_KEY_ID not set.");
        var secretKey = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_KEY_SECRET") ?? throw new InvalidOperationException("DIGITALOCEAN_SPACES_KEY_SECRET not set.");
        this.sessionBucket = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_SESSION_BUCKET") ?? throw new InvalidOperationException("DIGITALOCEAN_SPACES_SESSION_BUCKET not set.");
        this.spacesClient = new SpacesClient(endpoint, accessKey, secretKey);
    }
    #endregion

    #region Fields
    protected DigitalOceanClient client;
    protected ChatClient chatClient;
    protected ApiAgent apiAgent;
    protected AIAgent aiAgent;
    protected SpacesClient? spacesClient;
    protected string? sessionBucket;
    #endregion
}

public class DurableAgentSession : AgentSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public long VersionId { get; set; } = 0;

}