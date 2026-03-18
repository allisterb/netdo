namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class KBQuery
{
    public string? query { get; set; }
    public int num_results { get; set; }
    public KBFilters? filters { get; set; }
    public float alpha { get; set; }
}

public class KBFilters
{
    public KBOr_All[]? or_all { get; set; }
}

public class KBOr_All
{
    public KB_Starts_With? starts_with { get; set; }
    public KB_Greater_Than_Or_Equals? greater_than_or_equals { get; set; }
}

public class KB_Starts_With
{
    public string? key { get; set; }
    public string? value { get; set; }
}

public class KB_Greater_Than_Or_Equals
{
    public string? key { get; set; }
    public string? value { get; set; }
}

public class KBResults
{
    public Result[]? results { get; set; }
    public int total_results { get; set; }
}

public class Result
{
    public Metadata? metadata { get; set; }
    public string? text_content { get; set; }
}

public class Metadata
{
    public string? chunk_category { get; set; }
    public DateTime ingested_timestamp { get; set; }
    public string? item_name { get; set; }
}

public class KnowledgeBaseClient
{
    private readonly HttpClient _httpClient;

    public KnowledgeBaseClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        // Ensure the base address is set for the Knowledge Base API
        _httpClient.BaseAddress = new Uri("https://kbaas.do-ai.run");
    }

    /// <summary>
    /// Retrieves relevant chunks using hybrid retrieval (lexical + semantic).
    /// </summary>
    /// <param name="knowledgeBaseUuid">The UUID of the knowledge base.</param>
    /// <param name="query">The query object containing search parameters.</param>
    /// <returns>A KBResults object containing the search results.</returns>
    public async Task<KBResults?> Retrieve(string knowledgeBaseUuid, KBQuery query)
    {
        if (string.IsNullOrWhiteSpace(knowledgeBaseUuid))
        {
            throw new ArgumentException("Knowledge Base UUID cannot be null or empty.", nameof(knowledgeBaseUuid));
        }

        if (query == null)
        {
            throw new ArgumentNullException(nameof(query), "Query object cannot be null.");
        }

        var requestUri = $"/v1/{knowledgeBaseUuid}/retrieve";
        var jsonContent = JsonSerializer.Serialize(query, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUri, httpContent);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<KBResults>(responseBody);
    }
}