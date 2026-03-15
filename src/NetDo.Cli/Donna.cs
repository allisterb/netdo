namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;

using DigitalOcean.Api;

public class DonnaApi
{
    #region Methods
    /// <summary>
    /// List Agents
    /// </summary>
    /// <returns></returns>
    public ApiAgentPublic[]? ListAgents() => client.Genai_list_agentsAsync(null, null, null).GetAwaiter().GetResult()?.Agents?.ToArray();

    /// <summary>
    /// Retrieve an existing agent.
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public ApiAgent? GetAgent(string uuid) => client.Genai_get_agentAsync(uuid).GetAwaiter().GetResult()?.Agent;

    /// <summary>
    /// List All Projects
    /// </summary>
    /// <returns></returns>
    public Project[]? ListProjects() => client.Projects_listAsync(null, null).GetAwaiter().GetResult()?.Projects?.ToArray();

    /// <summary>
    /// List Workspaces
    /// </summary>
    /// <returns></returns>
    public ApiWorkspace[]? ListWorkspaces() => client.Genai_list_workspacesAsync().GetAwaiter().GetResult()?.Workspaces?.ToArray();

    /// <summary>
    /// List Available Models
    /// </summary>
    /// <returns></returns>
    public ApiModelPublic[]? ListModels() => client.Genai_list_modelsAsync(null, null, null, null).GetAwaiter().GetResult()?.Models?.ToArray();

    /// <summary>
    /// Get the current balance for the account.
    /// </summary>
    /// <returns></returns>
    public Balance? GetBalance() => client.Balance_getAsync().GetAwaiter().GetResult();
    #endregion

    #region Fields
    protected DigitalOceanClient client = new DigitalOceanClient();
    #endregion
}
