namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;
using Spectre.Console;
using Microsoft.Extensions.AI;

using DigitalOcean.Api;
using DigitalOcean.Gradient;
using NTokenizers.Extensions.Spectre.Console;
using RadLine;
using Microsoft.Agents.AI;

internal class Program : Runtime
{
    static Program()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIGITALOCEAN_API_TOKEN")))
        {
            AnsiConsole.MarkupLine("[red]The DIGITALOCEAN_API_TOKEN environment variable is not defined.[/]");
            Environment.Exit(1);
        }
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN")))
        {
            AnsiConsole.MarkupLine("[red]The GRADIENT_AGENT_API_TOKEN environment variable is not defined.[/]");
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
        var result = parser.ParseArguments<ProjectOptions, WorkspaceOptions, ModelOptions, AgentOptions, KBOptions, SpacesOptions, DonnaOptions, TestOptions>(args);
        try
        {
            await result.MapResult(
                async (ProjectOptions opts) => await HandleProjectsArgs(opts),
                async (WorkspaceOptions opts) => await HandleWorkspacesArgs(opts),
                async (ModelOptions opts) => await HandleModelArgs(opts),
                async (AgentOptions opts) => await HandleAgentArgs(opts),
                async (KBOptions opts) => await HandleKBArgs(opts),
                async (SpacesOptions opts) => await HandleSpacesArgs(opts),
                async (DonnaOptions opts) => await HandleDonnaArgs(opts),
                async (TestOptions opts) => await HandleTestArgs(opts),

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
        else if (!string.IsNullOrWhiteSpace(options.Fetch))
        {
            var response = await client.Genai_get_agentAsync(options.Fetch);
            var agent = response.Agent;

            if (agent == null)
            {
                AnsiConsole.MarkupLine($"[red]Agent with UUID '{options.Fetch}' not found.[/]");
                return;
            }
            var keys = await client.Genai_list_agent_api_keysAsync(agent.Uuid!, null, null);

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("[blue]Name:[/]", agent.Name ?? "");
            grid.AddRow("[blue]Agent ID:[/]", agent.Uuid ?? "");
            grid.AddRow("[blue]Project ID:[/]", agent.Project_id ?? "");
            grid.AddRow("[blue]Workspace ID:[/]", $"{agent.Workspace?.Name} ({agent.Workspace?.Uuid})");
            grid.AddRow("[blue]Description:[/]", agent.Description ?? "");
            grid.AddRow("[blue]Instruction:[/]", agent.Instruction ?? "");
            grid.AddRow("[blue]Model:[/]", $"{agent.Model?.Name} ({agent.Model?.Uuid})");
            grid.AddRow("[blue]Model Key:[/]", $"{agent.Model_provider_key?.Name} ({agent.Model_provider_key?.Api_key_uuid})");
            grid.AddRow("[blue]Created At:[/]", agent.Created_at?.ToString() ?? "");
            grid.AddRow("[blue]Updated At:[/]", agent.Updated_at?.ToString() ?? "");
            if (agent.Deployment != null)
            {
                grid.AddRow("", "");
                grid.AddRow("[blue]Deployment:[/]", "");
                grid.AddRow("  [blue]Name:[/]", agent.Deployment.Name ?? "");
                grid.AddRow("  [blue]Status:[/]", agent.Deployment.Status?.ToString() ?? "");
                grid.AddRow("  [blue]Visibility:[/]", agent.Deployment.Visibility?.ToString() ?? "");
                grid.AddRow("  [blue]UUID:[/]", agent.Deployment.Uuid ?? "");
                grid.AddRow("  [blue]URL:[/]", agent.Deployment.Url ?? "");
                grid.AddRow("  [blue]Created At:[/]", agent.Deployment.Created_at?.ToString() ?? "");
                grid.AddRow("  [blue]Updated At:[/]", agent.Deployment.Updated_at?.ToString() ?? "");
            }
            if (keys is not null && keys.Api_key_infos is not null && keys.Api_key_infos.Any())
            {
                grid.AddRow("", "");
                grid.AddRow("[blue]Agent APi Keys:[/]", "");
                foreach (var key in keys.Api_key_infos)
                {
                    grid.AddRow($"  [blue]Name:[/]", $"{key.Name ?? ""}");
                    grid.AddRow($"  [blue]Uuid:[/]", $"{key.Uuid ?? ""}");
                    grid.AddRow($"  [blue]Created at:[/]", $"{key.Created_at}");
                }
            }
            AnsiConsole.Write(new Panel(grid)
            {
                Header = new PanelHeader($"Agent Details: {agent.Name}"),
                Padding = new Padding(1, 1, 1, 1)
            });
        }
        else if (!string.IsNullOrWhiteSpace(options.CreateKey))
        {
            if (string.IsNullOrWhiteSpace(options.Name))
            {
                AnsiConsole.MarkupLine("[red]Error: --name is required for creating an agent API key.[/]");
                Environment.Exit(1);
            }

            var input = new ApiCreateAgentAPIKeyInputPublic(options.CreateKey, options.Name);
            var response = await client.Genai_create_agent_api_keyAsync(options.CreateKey, input);
            var keyInfo = response.Api_key_info;

            AnsiConsole.MarkupLine($"[green]API key '{keyInfo?.Name}' created successfully for agent: {options.CreateKey}[/]");
            AnsiConsole.MarkupLine($"[blue]UUID:[/] {keyInfo?.Uuid}");
            AnsiConsole.MarkupLine($"[yellow]Secret Key:[/] [bold]{keyInfo?.Secret_key}[/]");
            AnsiConsole.MarkupLine("[red]Warning: This is the only time the secret key will be displayed. Please save it securely.[/]");
        }
        else if (!string.IsNullOrWhiteSpace(options.ShowKey))
        {
            var response = await client.Genai_get_agentAsync(options.ShowKey);
            var agent = response.Agent;

            if (agent == null)
            {
                AnsiConsole.MarkupLine($"[red]Agent with UUID '{options.ShowKey}' not found.[/]");
                return;
            }

            var keysResponse = await client.Genai_list_agent_api_keysAsync(agent.Uuid!, null, null);
            var keys = keysResponse.Api_key_infos;

            if (keys == null || !keys.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]No API keys found for agent '{agent.Name}'.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[bold red]WARNING: You are about to display the secret API keys for this agent.[/]");
            AnsiConsole.MarkupLine("[bold red]Ensure you are in a private environment and no one is watching your screen.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Press [blue]any key[/] to continue or [blue]Ctrl+C[/] to cancel...");
            Console.ReadKey(true);

            var table = new Table();
            table.AddColumn("Key Name");
            table.AddColumn("UUID");
            table.AddColumn("Secret Key");

            foreach (var key in keys)
            {
                table.AddRow(
                    key.Name ?? "Unnamed",
                    key.Uuid ?? "",
                    $"[bold yellow]{key.Secret_key ?? "(masked/empty)"}[/]"
                );
            }

            AnsiConsole.Write(table);
        }
        else if (!string.IsNullOrWhiteSpace(options.DeployPublic))
        {
            var input = new ApiUpdateAgentDeploymentVisibilityInputPublic(options.DeployPublic, ApiDeploymentVisibility.VISIBILITY_PUBLIC);
            var response = await client.Genai_update_agent_deployment_visibilityAsync(options.DeployPublic, input);
            AnsiConsole.MarkupLine($"[green]Agent '{response.Agent?.Name}' (UUID: {options.DeployPublic}) deployment visibility set to public successfully.[/]");
        }
        else if (options.Create)
        {
            if (!string.IsNullOrEmpty(options.WorkspaceName))
            {
                var workspaceUuid = await GetWorkSpaceUuid(options.WorkspaceName);
                if (workspaceUuid is null)
                {
                    AnsiConsole.MarkupLine($"[red]Could not find workspace with name: {options.WorkspaceName}.[/]");
                    Environment.Exit(1);
                }
                options.WorkspaceUuid = workspaceUuid;
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

    static async Task HandleKBArgs(KBOptions options)
    {
        if (options.List)
        {
            var response = await client.Genai_list_knowledge_basesAsync(null, null);
            if (response.Knowledge_bases == null || !response.Knowledge_bases.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No knowledge bases found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Uuid");
            table.AddColumn("Region");

            foreach (var kb in response.Knowledge_bases)
            {
                table.AddRow(kb.Name ?? "", kb.Uuid ?? "", kb.Region ?? "");
            }
            AnsiConsole.Write(table);
        }
        else if (!string.IsNullOrWhiteSpace(options.Get))
        {
            var response = await client.Genai_get_knowledge_baseAsync(options.Get);
            var kb = response.Knowledge_base;
            if (kb == null)
            {
                AnsiConsole.MarkupLine($"[red]Knowledge base with UUID '{options.Get}' not found.[/]");
                return;
            }

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            grid.AddRow("[blue]Name:[/]", kb.Name ?? "");
            grid.AddRow("[blue]UUID:[/]", kb.Uuid ?? "");
            grid.AddRow("[blue]Region:[/]", kb.Region ?? "");
            grid.AddRow("[blue]Embedding Model:[/]", kb.Embedding_model_uuid ?? "");

            var dsResponse = await client.Genai_list_knowledge_base_data_sourcesAsync(kb.Uuid!, null, null);
            if (dsResponse.Knowledge_base_data_sources != null && dsResponse.Knowledge_base_data_sources.Any())
            {
                var dsTable = new Table().Border(TableBorder.None).HideHeaders();
                dsTable.AddColumn("Type");
                dsTable.AddColumn("Details");
                foreach (var ds in dsResponse.Knowledge_base_data_sources)
                {
                    string details = ds.Spaces_data_source != null 
                        ? $"Spaces: {ds.Spaces_data_source.Bucket_name} ({ds.Spaces_data_source.Region})" 
                        : ds.Uuid ?? "";
                    dsTable.AddRow(ds.Spaces_data_source != null ? "DO Space" : "Other", details);
                }
                grid.AddRow(new Markup("[blue]Data Sources:[/ binary]"), dsTable);
            }

            AnsiConsole.Write(new Panel(grid) { Header = new PanelHeader($"KB Details: {kb.Name}") });
        }
        else if (options.Create)
        {
            if (string.IsNullOrWhiteSpace(options.Name) || string.IsNullOrWhiteSpace(options.Region) || string.IsNullOrWhiteSpace(options.ProjectId) || string.IsNullOrWhiteSpace(options.EmbeddingModelUuid))
            {
                AnsiConsole.MarkupLine("[red]Error: --name, --region, --project-id, and --embedding-model-uuid are required for creating a knowledge base.[/]");
                return;
            }
            var input = new ApiCreateKnowledgeBaseInputPublic(
                database_id: options.DatabaseId,
                datasources: null,
                embedding_model_uuid: options.EmbeddingModelUuid,
                name: options.Name,
                project_id: options.ProjectId,
                region: options.Region,
                tags: options.Tags?.ToList(),
                vpc_uuid: options.VpcUuid
            );
            var response = await client.Genai_create_knowledge_baseAsync(input);
            AnsiConsole.MarkupLine($"[green]Knowledge base '{response.Knowledge_base?.Name}' created successfully with UUID: {response.Knowledge_base?.Uuid}[/]");
        }
        else if (options.ListEmbeddingModels)
        {
            var usecases = new List<Anonymous2> { Anonymous2.MODEL_USECASE_KNOWLEDGEBASE };
            var response = await client.Genai_list_modelsAsync(usecases, true, null, null);

            if (response.Models == null || !response.Models.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No embedding models found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Uuid");

            foreach (var model in response.Models)
            {
                table.AddRow(
                    model.Name ?? "",
                    model.Uuid ?? ""
                );
            }

            AnsiConsole.Write(table);
        }
        else if (options.AddDataSource)
        {
            if (string.IsNullOrWhiteSpace(options.Uuid) || string.IsNullOrWhiteSpace(options.BucketName) || string.IsNullOrWhiteSpace(options.Region))
            {
                AnsiConsole.MarkupLine("[red]Error: --uuid, --bucket-name, and --region are required to add a data source.[/]");
                return;
            }
            var dsInput = new ApiCreateKnowledgeBaseDataSourceInputPublic(
                aws_data_source: null,
                chunking_algorithm: null,
                chunking_options: null,
                knowledge_base_uuid: options.Uuid,
                spaces_data_source: new ApiSpacesDataSource(options.BucketName, options.ItemPath, options.Region),
                web_crawler_data_source: null
            );
            var response = await client.Genai_create_knowledge_base_data_sourceAsync(options.Uuid, dsInput);
            AnsiConsole.MarkupLine($"[green]Data source added successfully with UUID: {response.Knowledge_base_data_source?.Uuid}[/]");
        }
        else if (!string.IsNullOrWhiteSpace(options.DeleteDataSource))
        {
            if (string.IsNullOrWhiteSpace(options.Uuid))
            {
                AnsiConsole.MarkupLine("[red]Error: --uuid is required to remove a data source.[/]");
                return;
            }
            await client.Genai_delete_knowledge_base_data_sourceAsync(options.Uuid, options.DeleteDataSource);
            AnsiConsole.MarkupLine($"[green]Data source '{options.DeleteDataSource}' removed successfully.[/]");
        }
    }
    

    static async Task HandleSpacesArgs(SpacesOptions options)
    {
        if (options.ListBuckets)
        {
            var endpoint = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ENDPOINT");
            var accessKeyId = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_ID");
            var accessKeySecret = Environment.GetEnvironmentVariable("DIGITALOCEAN_SPACES_ACCESS_KEY_SECRET");

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(accessKeyId) || string.IsNullOrWhiteSpace(accessKeySecret))
            {
                AnsiConsole.MarkupLine("[red]Error: DIGITALOCEAN_SPACES_ENDPOINT, DIGITALOCEAN_SPACES_ACCESS_KEY_ID, and DIGITALOCEAN_SPACES_ACCESS_KEY_SECRET environment variables must be set.[/]");
                return;
            }

            var spacesClient = new SpacesClient(endpoint, accessKeyId, accessKeySecret);
            var buckets = spacesClient.ListBucketsAsync().GetAwaiter().GetResult();

            if (buckets == null || !buckets.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No buckets found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Bucket Name");

            foreach (var bucket in buckets)
            {
                table.AddRow(bucket);
            }
            AnsiConsole.Write(table);
        }
    }

    static async Task HandleTestArgs(TestOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Exec))
        {
            JSInterp.Execute(File.ReadAllText(options.Exec));
        }
    }

    static async Task HandleDonnaArgs(DonnaOptions options)
    {
        var agent = new Agent("37e2d5f9-183e-11f1-b074-4e013e2ddde4");
        var session = await agent.CreateSessionAsync();
        var editor = new LineEditor()
        {
            Prompt = new LineEditorPrompt("[green]Donna>[/]", "."),                       
        };
        editor.KeyBindings.Add<Quit>(ConsoleKey.Escape);
        editor.KeyBindings.Add<Help>(ConsoleKey.F1);
        while (true)
        {
            AnsiConsole.MarkupLine("[bold]Enter your query or '/exit' or '/quit' to end. Press F1 for help.[/]\n");
            var input = editor.ReadLine(Ct).GetAwaiter().GetResult();
            
            if (string.Equals(input, "/exit", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(input, "/quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }
            AgentResponse? _response = null; 
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Querying...", async ctx =>
                {
                    _response = await agent.RunAsync([new ChatMessage(ChatRole.User, input)], session);
                    if (_response.Messages.Count > 0)
                    {
                        ctx.Status("Evaluating...");
                    }
                });
            AgentResponse response = _response!;
            if (response.Usage is not null)
            {
                var usagePanel = new Panel($"Input tokens: {response.Usage.InputTokenCount}. Cached input tokens: {response.Usage.CachedInputTokenCount}. " +
                    $"Output tokens: {response.Usage.OutputTokenCount}. Reasoning tokens: {response.Usage.ReasoningTokenCount}.")
                    .Header("[bold]Usage[/]") 
                    .BorderColor(Color.Purple)   // Optional: Sets the border color
                    .RoundedBorder();
                AnsiConsole.Write(usagePanel);
            }



            foreach (var message in response!.Messages)
                {    
                        
                    if (message.Role == ChatRole.Assistant)
                    {
                        AnsiConsole.Console.WriteMarkdown(message.Text);
                        var codeBlocks = JSInterp.ExtractJSFromMarkdown(message.Text);
                        if (codeBlocks.Length > 0)
                        {
                                                          
                            foreach (var code in codeBlocks)
                            {
                                JSInterp.Execute(code);
                            }

                               
                        }
                       
                    }
                }
                
            
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
        AnsiConsole.Write(new FigletText("Digital Ocean\n").Color(Color.Blue));        
    }

    static void WriteDigitalOceanErrorException(DigitalOceanApiException<Api.Error> exception)
    {
        AnsiConsole.MarkupLine($"[red]Error: {exception.Result.Message}[/]");
        AnsiConsole.WriteException(exception);
    }

    static DigitalOceanClient client;
}

public sealed class Quit : LineEditorCommand
{
    public override void Execute(LineEditorContext context) => Environment.Exit(0);
}

public sealed class Help : LineEditorCommand
{
    public override void Execute(LineEditorContext context)
    {

    }
}
