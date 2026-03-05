namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
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

    [Option("name", Required = false, HelpText = "Name of the agent to create.")]
    public string Name { get; set; } = string.Empty;

    [Option("description", Required = false, HelpText = "Description of the agent to create.")]
    public string Description { get; set; } = string.Empty;

    [Option("workspace-name", Required = false, HelpText = "Name of the workspace to associate with.")]
    public string? WorkspaceName { get; set; } = null;

    [Option("workspace-uuid", Required = false, HelpText = "UUID of the workspace to associate with.")]
    public string? WorkspaceUuid { get; set; } = null;

    [Option("instruction", Required = false, HelpText = "Instructions for the agent.")]
    public string Instruction { get; set; } = string.Empty;

    [Option("model-uuid", Required = false, HelpText = "UUID of the model to use.")]
    public string ModelUuid { get; set; } = string.Empty;

    [Option("model-key-uuid", Required = false, HelpText = "UUID of the model key to use.")]
    public string ModelKeyUuid { get; set; } = string.Empty;
   
    [Option("project-uuid", Required = false, HelpText = "UUID of the project to associate with.")]
    public string ProjectUuid { get; set; } = string.Empty;

    [Option("region", Required = false, HelpText = "Region for the agent.")]
    public string Region { get; set; } = string.Empty;
}
