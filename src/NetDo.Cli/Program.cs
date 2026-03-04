namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;
using DigitalOcean.Api;
using Spectre.Console;

internal class Program : Runtime
{
    static async Task Main(string[] args)
    {
        PrintLogo();
        var parser = new Parser(with =>
        {
            with.CaseInsensitiveEnumValues = true;
            with.HelpWriter = null;
        });
        var result = parser.ParseArguments<ProjectOptions, WorkspaceOptions>(args);
        await result.MapResult(
            async (ProjectOptions opts) => await Projects(opts),
            async (WorkspaceOptions opts) => await Workspaces(opts),            
            errs => HandleParseError(result, errs)
        );
    }

    static async Task Projects(ProjectOptions opts)
    {        
        if (opts.List)
        {
            await ListProjects();
        }
    }

    static async Task ListProjects()
    {
        try
        {
            var client = new DigitalOceanClient();
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
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    static async Task Workspaces(WorkspaceOptions options)
    {
        
        if (options.List)
        {
           await ListWorkspaces();
        }
    }

    static async Task ListWorkspaces()
    {
        try
        {
            var client = new DigitalOceanClient();
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
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
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
}
