namespace NetDo.Cli;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;
using DigitalOcean.Api;
using Spectre.Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        PrintLogo();
        var parser = new Parser(with =>
        {
            with.CaseInsensitiveEnumValues = true;
            with.HelpWriter = null;
        });

        var result = parser.ParseArguments<ProjectOptions>(args);

        await result.MapResult(
            async (ProjectOptions opts) =>
            {
                if (opts.List)
                {
                    await RunProjectOptions(opts);
                }
                else
                {
                    await HandleParseError(result, Enumerable.Empty<CommandLine.Error>());
                }
            },
            errs => HandleParseError(result, errs)
        );
    }

    static async Task RunProjectOptions(ProjectOptions opts)
    {
        if (opts.Debug)
        {
            DigitalOceanClient.DebugEnabled = true;
        }

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
        AnsiConsole.Write(new FigletText("NetDo"));
    }
}
