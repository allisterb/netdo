namespace DigitalOcean.Cli;

using DigitalOcean.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

[McpServerToolType]
public static class DonnaMCPTools
{    
    
    
    [McpServerTool, Description("Search the Donna API specification")]
    public static async Task<KBResults?> Search(string query)
    {
        return await kbclient.Retrieve(_kbuuid!, new KBQuery()
        {
            query = query,
            num_results = 10
        });
        
    }
   
    [McpServerTool, Description("Execute JavaScript code against the Donna API.")]
    public static string ExecuteJavaScript(string script)
    {
        return JSInterp.MCPExecute(script);
    }

    public static readonly KnowledgeBaseClient kbclient = new KnowledgeBaseClient();

    

    public static string? _kbuuid;
}

public class DonnaMCPServer : Runtime
{
    public static async Task Run()
    {        
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Add MCP server
        builder            
            .Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            ;
                        
        var app = builder.Build();

        await app.RunAsync();
    }
}
