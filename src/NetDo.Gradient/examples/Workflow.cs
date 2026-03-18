using System;
using System.ComponentModel;
using System.ClientModel;

using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

using DigitalOcean.Gradient;

public class Examples
{
    public async Task WorkflowExample1()
    {
        // Common Agent Platform data
        const string project_id = "";
        const string region = "";
        const string workspace_id = "";
        const string model_uuid = "";


        // Define agent configurations
        const string ReviewerAgentName = "Concierge";
        const string ReviewerAgentInstructions = @"
    You are an are hotel concierge who has opinions about providing the most local and authentic experiences for travelers.
    The goal is to determine if the front desk travel agent has recommended the best non-touristy experience for a traveler.
    If so, state that it is approved.
    If not, provide insight on how to refine the recommendation without using a specific example. 
    ";

        const string FrontDeskAgentName = "FrontDesk";
        const string FrontDeskAgentInstructions = @"
    You are a Front Desk Travel Agent with ten years of experience and are known for brevity as you deal with many customers.
    The goal is to provide the best activities and locations for a traveler to visit.
    Only provide a single recommendation per response.
    You're laser focused on the goal at hand.
    Don't waste time with chit chat.
    Consider suggestions when refining an idea.
    ";

        // Create AI agents
        Agent reviewerAgent = await Agent.CreateAsync(ReviewerAgentName, ReviewerAgentInstructions, model_uuid, project_id, region, workspace_id) ?? throw new Exception("Could not create reviewer agent");
        Agent frontDeskAgent = await Agent.CreateAsync(FrontDeskAgentName, FrontDeskAgentInstructions, model_uuid, project_id, region, workspace_id) ?? throw new Exception("Could not create front desk agent");

        // Alternativly retrieve existing agents from Agent Platform
        //Agent reviewerAgent = new Agent("rauuid");
        //Agent frontDeskAgent = new Agent("fduuid");

        
        // Build workflow
        var workflow = new WorkflowBuilder(frontDeskAgent)
            .AddEdge(frontDeskAgent, reviewerAgent)
            .Build();

        // Create user message
        ChatMessage userMessage = new ChatMessage(ChatRole.User, [
            new TextContent("I would like to go to Paris.")
        ]);

        // Execute workflow
        StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, userMessage);

        // Process workflow events
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

        string messageData = "";

        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is AgentResponseUpdateEvent executorComplete)
            {
                messageData += executorComplete.Data;
                Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            }
        }

        Console.WriteLine("\n=== Final Output ===");
        Console.WriteLine(messageData);

        // Mermaid
        Console.WriteLine("\nMermaid string: \n=======");
        var mermaid = workflow.ToMermaidString();
        Console.WriteLine(mermaid);
        Console.WriteLine("=======");

        // DOT - Save to file instead of stdout to avoid pipe issues
        var dotString = workflow.ToDotString();
        var dotFilePath = "workflow.dot";
        File.WriteAllText(dotFilePath, dotString);
        Console.WriteLine($"\nDOT graph saved to: {dotFilePath}");
        Console.WriteLine("To generate image: dot -Tsvg workflow.dot -o workflow.svg");
        Console.WriteLine("                   dot -Tpng workflow.dot -o workflow.png");

        // Console.WriteLine(messageData);
    }

}