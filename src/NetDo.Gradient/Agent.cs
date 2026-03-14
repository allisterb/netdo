namespace DigitalOcean.Gradient;

using DigitalOcean.Api;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
    protected override ValueTask<AgentSession> CreateSessionCoreAsync(CancellationToken cancellationToken) => this.aiAgent.CreateSessionAsync(cancellationToken);
        
    protected override Task<AgentResponse> RunCoreAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)    
        => this.aiAgent.RunAsync(messages, session, options, cancellationToken);

    protected override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null, CancellationToken cancellationToken = default)
        => this.aiAgent.RunStreamingAsync(messages, session, options, cancellationToken);

    protected override ValueTask<JsonElement> SerializeSessionCoreAsync(AgentSession session, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(JsonElement serializedState, JsonSerializerOptions? jsonSerializerOptions = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Fields
    protected DigitalOceanClient client;
    protected ChatClient chatClient;
    protected ApiAgent apiAgent;
    protected AIAgent aiAgent;
    #endregion
}
