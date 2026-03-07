namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;
using Spectre.Console;

using DigitalOcean.Api;

internal class Program : Runtime
{
    static Program()
    {
        if (Environment.GetEnvironmentVariable("DIGITALOCEAN_API_TOKEN") is null)
        {
            AnsiConsole.MarkupLine("[red]The DIGITALOCEAN_API_TOKEN environment variable is not defined.[/]");
            Environment.Exit(1);
        }
        client = new DigitalOceanClient();
    }
    
    static async Task Main(string[] args)
    {
        PrintLogo();
        var parser = new Parser(with =>
        {
            with.CaseInsensitiveEnumValues = true;
            with.HelpWriter = null;
        });
        var result = parser.ParseArguments<ProjectOptions, WorkspaceOptions, ModelOptions, AgentOptions>(args);
        try
        {
            await result.MapResult(
                async (ProjectOptions opts) => await HandleProjectsArgs(opts),
                async (WorkspaceOptions opts) => await HandleWorkspacesArgs(opts),
                async (ModelOptions opts) => await HandleModelArgs(opts),
                async (AgentOptions opts) => await HandleAgentArgs(opts),
                errs => HandleParseError(result, errs)
            );
        }
        catch (DigitalOceanApiException<Api.Error> ex)
        {
            WriteDigitalOceanErrorException(ex);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex); 
        }
    }

    static async Task HandleProjectsArgs(ProjectOptions opts)
    {             
        if (opts.List)
        {
            var response = await client.Projects_listAsync(null, null);

            if (response.Projects == null || !response.Projects.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No projects found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("ID");
            table.AddColumn("Description");
            table.AddColumn("Is Default");
            table.AddColumn("Created At");

            foreach (var project in response.Projects)
            {
                table.AddRow(
                    project.Name ?? "",
                    project.Id?.ToString() ?? "",
                    project.Description ?? "",
                    project.Is_default?.ToString() ?? "False",
                    project.Created_at?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
        
    }
    
    static async Task HandleWorkspacesArgs(WorkspaceOptions options)
    {        
        if (options.List)
        {
            var response = await client.Genai_list_workspacesAsync();

            if (response.Workspaces == null || !response.Workspaces.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No workspaces found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Uuid");
            table.AddColumn("Description");
            table.AddColumn("Agents");

            foreach (var workspace in response.Workspaces)
            {
                var agentsList = workspace.Agents != null
                    ? string.Join(", ", workspace.Agents.Select(a => $"{a.Name}({a.Uuid})"))
                    : "";

                table.AddRow(
                    workspace.Name ?? "",
                    workspace.Uuid ?? "",
                    workspace.Description ?? "",
                    agentsList
                );
            }

            AnsiConsole.Write(table);
        }        
    }

    static async Task HandleModelArgs(ModelOptions options)
    {
        try
        {
            if (options.List)
            {
                var response = await client.Genai_list_modelsAsync(null, null, null, null);

                if (response.Models == null || !response.Models.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]No models found.[/]");
                    return;
                }

                var table = new Table();
                table.AddColumn("Name");
                table.AddColumn("Uuid");
                table.AddColumn("Version");
                table.AddColumn("Updated at");

                foreach (var model in response.Models)
                {
                    var version = model.Version?.Major?.ToString() ?? "0" + "." + model.Version?.Minor?.ToString() ?? "0" + "." + model.Version?.Patch?.ToString() ?? "0";
                    table.AddRow(
                        model.Name ?? "",
                        model.Uuid ?? "",
                        version ?? "",
                        model.Updated_at?.ToString() ?? ""
                    );
                }
                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    static async Task HandleAgentArgs(AgentOptions options)
    {      
        if (options.List)
        {
            var response = await client.Genai_list_agentsAsync(null, null, null);

            if (response.Agents == null || !response.Agents.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No agents found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Description");
            table.AddColumn("Uuid");
            table.AddColumn("Model Uuid");
            table.AddColumn("Project Uuid");
            table.AddColumn("Route Uuid");

            foreach (var agent in response.Agents)
            {
                table.AddRow(
                    agent.Name ?? "",
                    agent.Description ?? "",
                    agent.Uuid ?? "",
                    agent.Model?.Uuid ?? "",
                    agent.Project_id ?? "",
                    agent.Route_uuid ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
        else if (options.Create)
        {
            if (!string.IsNullOrEmpty(options.WorkspaceName))
            {

                options.WorkspaceUuid = await GetWorkSpaceUuid(options.WorkspaceName);
                if (options.WorkspaceUuid is null)
                {
                    AnsiConsole.MarkupLine($"[red]Could not find workspace with name: {options.WorkspaceName}.[/]");
                    Environment.Exit(1);
                }
            }
            if (string.IsNullOrWhiteSpace(options.Name) ||
                string.IsNullOrWhiteSpace(options.Instructions) ||
                string.IsNullOrWhiteSpace(options.ModelUuid) ||
                string.IsNullOrWhiteSpace(options.ProjectUuid) ||
                string.IsNullOrWhiteSpace(options.Region)
                )
            {
                AnsiConsole.MarkupLine("[red]Error: Name, Instructions, ModelUuid, ProjectUuid and Region are all required for creating an agent.[/]");
                Environment.Exit(1);
            }

            var input = new ApiCreateAgentInputPublic(
                anthropic_key_uuid: null,
                description: options.Description,
                instruction: options.Instructions,
                knowledge_base_uuid: null,
                model_provider_key_uuid: null,
                model_uuid: options.ModelUuid,
                name: options.Name,
                open_ai_key_uuid: null,
                project_id: options.ProjectUuid,
                region: options.Region,
                tags: null,
                workspace_uuid: options.WorkspaceUuid

            );

            var response = await client.Genai_create_agentAsync(input);
            AnsiConsole.MarkupLine($"[green]Agent '{response.Agent?.Name}' created successfully with UUID: {response.Agent?.Uuid}[/]");
        }
    }

    static async Task<string?> GetWorkSpaceUuid(string name)
    {
        var workspaces = await client.Genai_list_workspacesAsync();
        var w = workspaces.Workspaces?.FirstOrDefault(_w => _w.Name == name);
        return w?.Uuid;
    }
 
    static Task HandleParseError<T>(ParserResult<T> result, IEnumerable<CommandLine.Error> errs)
    {
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.AddOptions(result);
            return h;
        },
        e =>
        {
            return e;
        });
        Console.WriteLine(helpText);
        return Task.CompletedTask;
    }

    static void PrintLogo()
    {
        AnsiConsole.Write(new FigletText("Digital Ocean").Color(Color.Blue));
    }

    static void WriteDigitalOceanErrorException(DigitalOceanApiException<Api.Error> exception)
    {
        AnsiConsole.MarkupLine($"[red]Error: {exception.Result.Message}[/]");
        AnsiConsole.WriteException(exception);
    }

    static DigitalOceanClient client;
}
