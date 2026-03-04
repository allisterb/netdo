using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalOcean.Gradient;

#region Enums
public enum ModelRuntime
{
    OpenAI,
    Anthropic
}

public enum ModelProvider
{
    OpenAI,
    Anthropic
}
#endregion

#region Records
public record EvaluationResult(
    string RunUuid,
    string Status,
    bool? Passed,
    string? StarMetricName,
    double? StarMetricValue,
    string? StarMetricReasoning,
    IDictionary<string, double>? RunLevelMetrics
);
#endregion
