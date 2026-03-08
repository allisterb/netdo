namespace DigitalOcean.Gradient;

using DigitalOcean.Api;
using Microsoft.Extensions.AI;
using OpenAI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class Agent : DelegatingChatClient
{
    #region Constructors
    private Agent(ApiAgent _agent) : base(GetOpenAIChatClientForApiAgent(_agent))
    {
        this._agent = _agent;
        this.client = new DigitalOceanClient();
        ///this.InnerClient.
        //this.openAIClient = (OpenAIClient)this.InnerClient.;
    }

    public Agent(string uuid) : this(GetAgent(uuid)) {}
    #endregion

    #region Properties
    public string Id => _agent.Uuid!;

    #endregion

    #region Static Methods
    public static ApiAgent GetAgent(string uuid)
    {
        var client = new DigitalOceanClient();
        try
        {
            var agent = client.Genai_get_agentAsync(uuid).GetAwaiter().GetResult();
            if (agent is not null && agent.Agent is not null)
            {
                return agent.Agent;
            }
            else
            {
                throw new Exception($"An unknown error occurred attempting to retrieve agent with uuid {uuid}.");
            }
        }
        catch (DigitalOceanApiException<Error> e)
        {
            throw new Exception($"An error occurred attempting to retrieve agent with uuid {uuid}: {e.Result.Message}.", e);
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred attempting to retrieve agent with uuid {uuid}.", e);
        }
    }

    public static IChatClient GetOpenAIChatClientForApiAgent(ApiAgent agent)
    {
        var endpoint = agent.Deployment?.Url ?? throw new ArgumentNullException($"The agent {agent.Uuid} ({agent.Name}) deployment field or deployment url is null.");
        var apikey = Environment.GetEnvironmentVariable("GRADIENT_AGENT_API_TOKEN") ?? throw new ArgumentNullException("The GRADIENT_AGENT_API_TOKEN environment viariable is not set.");
        return 
            new OpenAIClient(new System.ClientModel.ApiKeyCredential(apikey), new OpenAIClientOptions() { Endpoint = new Uri(endpoint + "/api/v1/chat/completions") })
            .GetChatClient(agent.Model!.Inference_name)           
            .AsIChatClient()
            .AsBuilder()
            .UseLogging(Runtime.loggerFactory)
            .UseFunctionInvocation(Runtime.loggerFactory)
            .Build();
    }

    public static async Task<IEnumerable<ApiAgentPublic>> ListAsync(bool onlyDeployed = false, CancellationToken ct = default)
    {
        var client = new DigitalOceanClient();
        var response = await client.Genai_list_agentsAsync(onlyDeployed, null, null, ct);
        return response.Agents ?? Enumerable.Empty<ApiAgentPublic>();
    }

    public static async Task<ApiAgent?> GetAsync(string id, CancellationToken ct = default)
    {
        var client = new DigitalOceanClient();
        var response = await client.Genai_get_agentAsync(id, ct);
        return response.Agent;
    }

    public static async Task<Agent> CreateAsync(
        string name,
        ModelProvider provider,
        string modelUuid,
        string? description = null,
        string? instruction = null,
        string? openAiKeyUuid = null,
        string? anthropicKeyUuid = null,
        string? projectId = null,
        CancellationToken ct = default)
    {
        var client = new DigitalOceanClient();
        var input = new ApiCreateAgentInputPublic(
            anthropic_key_uuid: anthropicKeyUuid,
            description: description,
            instruction: instruction,
            knowledge_base_uuid: null,
            model_provider_key_uuid: null,
            model_uuid: modelUuid,
            name: name,
            open_ai_key_uuid: openAiKeyUuid,
            project_id: projectId,
            region: null,
            tags: null,
            workspace_uuid: null
        );

        var response = await client.Genai_create_agentAsync(input, ct);
        return new Agent(response.Agent!);
    }

    #endregion

    #region Instance Methods
    public async Task<ChatResponse> PromptAsync(string prompt, params object[] content)
    {               
        var messageItems = new List<AIContent>()
        {
            new TextContent(prompt)
        };
        if (content is not null)
        {
            foreach (var item in content)
            {
                if (item is string s)
                {
                    messageItems.Add(new TextContent(s));
                }               
                else
                {
                    throw new ArgumentException($"Unsupported content type {item.GetType()}");
                }
            }
        }        
        return await this.GetResponseAsync(new ChatMessage(ChatRole.User, messageItems));              
    }

    public async Task RefreshMetadataAsync(CancellationToken ct = default)
    {
        var response = await client.Genai_get_agentAsync(Id, ct);
        _agent = response.Agent;
    }

    public async Task DeleteAsync(CancellationToken ct = default)
    {
        await client.Genai_delete_agentAsync(Id, ct);
    }

    public async Task<ApiGetAgentUsageOutput> GetUsageAsync(DateTimeOffset? start = null, DateTimeOffset? stop = null, CancellationToken ct = default)
    {
        return await client.Genai_get_agent_usageAsync(Id, start?.ToString("O"), stop?.ToString("O"), ct);
    }
    /*
    public async Task<EvaluationResult> RunEvaluationAsync(
        string testCaseName,
        string datasetFilePath,
        IEnumerable<string> metricCategories,
        string? starMetricName = null,
        double? successThreshold = null,
        CancellationToken ct = default)
    {
        // 1. Validate Dataset
        ValidateDataset(datasetFilePath);

        // 2. Upload Dataset
        string datasetUuid = await UploadDatasetAsync(datasetFilePath, $"{testCaseName}_dataset", ct);

        // 3. Find/Create Test Case
        string testCaseUuid = await GetOrCreateTestCaseAsync(testCaseName, datasetUuid, metricCategories, starMetricName, successThreshold, ct);

        // 4. Run Evaluation
        var runInput = new ApiRunEvaluationTestCaseInputPublic(
            agent_deployment_names: null, // Used for ADK workspaces, we use agent_uuids
            agent_uuids: new[] { Id },
            run_name: $"{testCaseName}_run",
            test_case_uuid: testCaseUuid
        );

        var runResponse = await _client.Genai_run_evaluation_test_caseAsync(runInput, ct);
        string runUuid = runResponse.Evaluation_run_uuids!.First();

        // 5. Poll for completion
        var result = await PollEvaluationRunAsync(runUuid, ct);
        return result;
    }
    #endregion

    #region Private Methods
    private void ValidateDataset(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Dataset file not found", filePath);
        if (Path.GetExtension(filePath).ToLower() != ".csv") throw new ArgumentException("Dataset must be a CSV file");

        // Basic CSV validation (simplified)
        using var reader = new StreamReader(filePath);
        string? header = reader.ReadLine();
        if (header == null) throw new ArgumentException("CSV file is empty");

        var columns = header.Split(',').Select(c => c.Trim('"')).ToList();
        int queryIndex = columns.IndexOf("query");
        if (queryIndex == -1) throw new ArgumentException("Missing required column: 'query'");

        int rowNum = 2;
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            // This is a naive split, but sufficient for basic validation of the query column
            // In a real scenario, a proper CSV parser should be used.
            var fields = line.Split(','); 
            if (fields.Length > queryIndex)
            {
                var queryVal = fields[queryIndex].Trim('"');
                try
                {
                    JToken.Parse(queryVal);
                }
                catch (JsonException ex)
                {
                    throw new ArgumentException($"Invalid JSON in 'query' column at row {rowNum}: {ex.Message}");
                }
            }
            rowNum++;
        }
    }

    
    private async Task<string> UploadDatasetAsync(string filePath, string datasetName, CancellationToken ct)
    {
        var fileInfo = new FileInfo(filePath);
        var presignedInput = new ApiCreateDataSourceFileUploadPresignedUrlsInputPublic(
            files: new[] { new ApiPresignedUrlFile(fileInfo.Name, fileInfo.Length) }
        );

        var presignedResponse = await _client.Genai_create_evaluation_dataset_file_upload_presigned_urlsAsync(presignedInput, ct);
        var uploadInfo = presignedResponse.Uploads!.First();

        using (var httpClient = new HttpClient())
        using (var fileStream = File.OpenRead(filePath))
        {
            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            var response = await httpClient.PutAsync(uploadInfo.Presigned_url, content, ct);
            response.EnsureSuccessStatusCode();
        }

        var datasetInput = new ApiCreateEvaluationDatasetInputPublic(
            file_upload_dataset: new ApiFileUploadDataSource(fileInfo.Name, fileInfo.Length, uploadInfo.Object_key),
            name: datasetName
        );

        var datasetResponse = await _client.Genai_create_evaluation_datasetAsync(datasetInput, ct);
        return datasetResponse.Evaluation_dataset_uuid!;
    }

    private async Task<string> GetOrCreateTestCaseAsync(
        string name,
        string datasetUuid,
        IEnumerable<string> metricCategories,
        string? starMetricName,
        double? successThreshold,
        CancellationToken ct)
    {
        // For simplicity, we'll list test cases and find by name. 
        // Note: The API might require workspace_uuid or agent_workspace_name which we might not have for managed agents.
        // Managed agents might not be compatible with the current evaluation API if it strictly requires "agent workspaces".
        // However, the Python ADK uses it, so we'll try.
        
        var metricsResponse = await _client.Genai_list_evaluation_metricsAsync(ct);
        var selectedMetrics = metricsResponse.Metrics!
            .Where(m => metricCategories.Any(c => m.Category.ToString()!.Contains(c, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (!selectedMetrics.Any()) throw new ArgumentException("No metrics found for specified categories");

        var starMetricObj = starMetricName != null 
            ? selectedMetrics.FirstOrDefault(m => m.Metric_name == starMetricName) 
            : selectedMetrics.First();

        if (starMetricObj == null) throw new ArgumentException($"Star metric '{starMetricName}' not found in selected categories");

        var starMetric = new ApiStarMetric(starMetricObj.Metric_uuid, starMetricObj.Metric_name, successThreshold);

        // Try to find existing
        var testCasesResponse = await _client.Genai_list_evaluation_test_casesAsync(ct);
        var existing = testCasesResponse.Evaluation_test_cases?.FirstOrDefault(tc => tc.Name == name);

        if (existing != null)
        {
            var updateInput = new ApiUpdateEvaluationTestCaseInputPublic(
                dataset_uuid: datasetUuid,
                description: null,
                metrics: new ApiUpdateEvaluationTestCaseMetrics(selectedMetrics.Select(m => m.Metric_uuid!).ToList()),
                name: null,
                star_metric: starMetric
            );
            await _client.Genai_update_evaluation_test_caseAsync(existing.Test_case_uuid!, updateInput, ct);
            return existing.Test_case_uuid!;
        }
        else
        {
            var createInput = new ApiCreateEvaluationTestCaseInputPublic(
                agent_workspace_name: null,
                dataset_uuid: datasetUuid,
                description: $"Evaluation for agent {Id}",
                metrics: selectedMetrics.Select(m => m.Metric_uuid!).ToList(),
                name: name,
                star_metric: starMetric,
                workspace_uuid: null // May need to be set if required by API
            );
            var createResponse = await _client.Genai_create_evaluation_test_caseAsync(createInput, ct);
            return createResponse.Test_case_uuid!;
        }
    }
    */
    private async Task<EvaluationResult> PollEvaluationRunAsync(string runUuid, CancellationToken ct)
    {
        while (true)
        {
            var response = await client.Genai_get_evaluation_runAsync(runUuid, ct);
            var run = response.Evaluation_run!;

            if (run.Status == ApiEvaluationRunStatus.EVALUATION_RUN_SUCCESSFUL ||
                run.Status == ApiEvaluationRunStatus.EVALUATION_RUN_FAILED ||
                run.Status == ApiEvaluationRunStatus.EVALUATION_RUN_CANCELLED ||
                run.Status == ApiEvaluationRunStatus.EVALUATION_RUN_PARTIALLY_SUCCESSFUL)
            {
                return new EvaluationResult(
                    RunUuid: runUuid,
                    Status: run.Status.ToString()!,
                    Passed: run.Pass_status,
                    StarMetricName: run.Star_metric_result?.Metric_name,
                    StarMetricValue: run.Star_metric_result?.Number_value,
                    StarMetricReasoning: run.Star_metric_result?.Reasoning,
                    RunLevelMetrics: run.Run_level_metric_results?.ToDictionary(m => m.Metric_name!, m => m.Number_value ?? 0)
                );
            }

            await Task.Delay(5000, ct);
        }
    }
    #endregion

    #region Fields
    protected DigitalOceanClient client;
    protected ApiAgent _agent;
    //protected OpenAIClient openAIClient;
    #endregion
}
