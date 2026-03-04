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
    [Option("list", Required = false, HelpText = "List all workspaces.")]
    public bool List { get; set; }
}

