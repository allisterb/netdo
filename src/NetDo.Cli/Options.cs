namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CommandLine;

public class Options
{
    [Option("debug", Required = false, HelpText = "Enable debug mode.")]
    public bool Debug { get; set; }
   
    [Option("options", Required = false, HelpText = "Any additional options for the selected operation.")]
    public string AdditionalOptions { get; set; } = String.Empty;

    public static Dictionary<string, object> Parse(string o)
    {
        Dictionary<string, object> options = new Dictionary<string, object>();
        Regex re = new Regex(@"(\w+)\=([^\,]+)", RegexOptions.Compiled);
        string[] pairs = o.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in pairs)
        {
            Match m = re.Match(s);
            if (!m.Success)
            {
                options.Add("_ERROR_", s);
            }
            else if (options.ContainsKey(m.Groups[1].Value))
            {
                options[m.Groups[1].Value] = m.Groups[2].Value;
            }
            else
            {
                options.Add(m.Groups[1].Value, m.Groups[2].Value);
            }
        }
        return options;
    }
}


[Verb("projects", HelpText = "View and manage Digital Ocean projects.")]
public class ProjectOptions : Options
{
    [Option("list", Required = false, HelpText = "List all projects.")]
    public bool List { get; set; }
}

[Verb("workspaces", HelpText = "View and manage Digital Ocean agent workspaces.")]
public class WorkspaceOptions : Options
{
    [Option("list", Required = false, HelpText = "List all workspaces.", SetName = "workspace_options")]
    public bool List { get; set; }

    [Option("create", Required = false, HelpText = "Create a workspace.", SetName = "workspace_options")]
    public string Create { get; set; } = "";
}

[Verb("models", HelpText = "View and manage Digital Ocean agent models.")]
public class ModelOptions : Options
{
    [Option("list", Required = false, HelpText = "List all models.", SetName = "model_options")]
    public bool List { get; set; }    
}

[Verb("agents", HelpText = "View and manage Digital Ocean agents.")]
public class AgentOptions : Options
{
    [Option("list", Required = false, HelpText = "List all agents.", SetName = "agent_options")]
    public bool List { get; set; }

    [Option("create", Required = false, HelpText = "Create an agent.", SetName = "agent_options")]
    public bool Create { get; set; }

    [Option("create-key", Required = false, HelpText = "Create an API key for the agent with the specified UUID.", SetName = "agent_options")]
    public string CreateKey { get; set; } = string.Empty;

    [Option("fetch", Required = false, HelpText = "Fetch details of an agent by UUID.", SetName = "agent_options")]
    public string Fetch { get; set; } = string.Empty;

    [Option("name", Required = false, HelpText = "Name of the agent.")]
    public string Name { get; set; } = string.Empty;

    [Option("description", Required = false, HelpText = "Description of the agent.")]
    public string Description { get; set; } = string.Empty;

    [Option("instructions", Required = false, HelpText = "Instructions for the agent.")]
    public string Instructions { get; set; } = string.Empty;

    [Option("model-uuid", Required = false, HelpText = "UUID of the model to use.")]
    public string ModelUuid { get; set; } = string.Empty;

    [Option("model-key-uuid", Required = false, HelpText = "UUID of the model key to use.")]
    public string ModelKeyUuid { get; set; } = string.Empty;

    [Option("project-uuid", Required = false, HelpText = "UUID of the project to associate with.")]
    public string ProjectUuid { get; set; } = string.Empty;

    [Option("workspace-uuid", Required = false, HelpText = "UUID of the workspace to associate with.")]
    public string WorkspaceUuid { get; set; } = string.Empty;

    [Option("workspace-name", Required = false, HelpText = "Name of the workspace to associate with.")]
    public string WorkspaceName { get; set; } = string.Empty;

    [Option("region", Required = false, HelpText = "Region for the agent.")]
    public string Region { get; set; } = string.Empty;
}

[Verb("kb", HelpText = "Display commands that manage DigitalOcean Agent Knowledge Bases.")]
public class KBOptions : Options
{
// Subcommands
[Option("list", Required = false, HelpText = "List all knowledge bases for agents.", SetName = "kb_options")]
public bool List { get; set; }

[Option("get", Required = false, HelpText = "Retrieves a Knowledge Base by its uuid.", SetName = "kb_options")]
public string Get { get; set; } = string.Empty;

[Option("create", Required = false, HelpText = "Creates a knowledge base.", SetName = "kb_options")]
public bool Create { get; set; }

[Option("list-embedding-models", Required = false, HelpText = "List all available embedding models and their UUIDs.", SetName = "kb_options")]
public bool ListEmbeddingModels { get; set; }

[Option("add-datasource", Required = false, HelpText = "Add one datasource for knowledge base.", SetName = "kb_options")]
public bool AddDataSource { get; set; }
    [Option("delete-datasource", Required = false, HelpText = "Delete a datasource for knowledge base using its id.", SetName = "kb_options")]
    public string DeleteDataSource { get; set; } = string.Empty;

    // Flags
    [Option("name", Required = false, HelpText = "The name of the Knowledge Base.")]
    public string Name { get; set; } = string.Empty;

    [Option("region", Required = false, HelpText = "The region of the Knowledge Base.")]
    public string Region { get; set; } = string.Empty;

    [Option("project-id", Required = false, HelpText = "The project ID of the Knowledge Base.")]
    public string ProjectId { get; set; } = string.Empty;

    [Option("embedding-model-uuid", Required = false, HelpText = "The embedding model UUID of the Knowledge Base.")]
    public string EmbeddingModelUuid { get; set; } = string.Empty;

    [Option("database-id", Required = false, HelpText = "The database ID of the Knowledge Base.")]
    public string DatabaseId { get; set; } = string.Empty;

    [Option("tags", Required = false, HelpText = "The tags of the Knowledge Base. Example: --tags tag1,tag2,tag3")]
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

    [Option("vpc_uuid", Required = false, HelpText = "The VPC UUID of the Knowledge Base.")]
    public string VpcUuid { get; set; } = string.Empty;

    [Option("uuid", Required = false, HelpText = "The UUID of the Knowledge Base.")]
    public string Uuid { get; set; } = string.Empty;

    // Data source flags
    [Option("bucket-name", Required = false, HelpText = "The bucket name of data source from Spaces")]
    public string BucketName { get; set; } = string.Empty;

    [Option("item-path", Required = false, HelpText = "Item path of data source from Spaces.")]
    public string ItemPath { get; set; } = string.Empty;
}
