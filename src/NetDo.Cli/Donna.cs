namespace DigitalOcean.Cli;

using System;
using System.Collections.Generic;
using System.Linq;

using DigitalOcean.Api;

public class DonnaApi
{
    #region Methods

    #region Account and Billing
    /// <summary>
    /// Retrieve the balances on the current customer's account.
    /// </summary>
    /// <returns></returns>
    public Balance? GetBalance() => client.Balance_getAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Get current user account billing history entry.
    /// </summary>
    /// <returns></returns>
    public Response21? ListBillingHistory() => client.BillingHistory_listAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Get current user account information.
    /// </summary>
    /// <returns></returns>
    public Response3? GetAccount() => client.Account_getAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Retrieve a list of all invoices for the current customer.
    /// </summary>
    /// <returns></returns>
    public Response22? ListInvoices() => client.Invoices_listAsync(null, null).GetAwaiter().GetResult();

    /// <summary>
    /// Retrieve an invoice s.
    /// </summary>
    /// <returns></returns>
    public Invoice_summary? GetInvoice(string uuid) => client.Invoices_get_summaryByUUIDAsync(uuid).GetAwaiter().GetResult();
    #endregion

    #region Gradient AI
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
    #endregion

    #region Apps
    public App[]? ListApps() => client.Apps_listAsync(null, null, null).GetAwaiter().GetResult()?.Apps?.ToArray();

    public App? GetAppById(string id) => client.Apps_getAsync(id, null).GetAwaiter().GetResult().App;

    public App? GetAppByName(string name) => client.Apps_getAsync("", name).GetAwaiter().GetResult().App;

    public App_health? GetAppHealth(string id) => client.Apps_get_healthAsync(id).GetAwaiter().GetResult().App_health;
    #endregion

    #region Databases
    public Database_cluster_read[]? ListDatabaseClusters() => client.Databases_list_clustersAsync(null).GetAwaiter().GetResult().Databases?.ToArray();
    
    public Opensearch_index[]? ListOpenSearchIndexes(string id) => client.Databases_list_opeasearch_indexesAsync(new Guid(id)).GetAwaiter().GetResult()?.Indexes?.ToArray();

    public Database_cluster_read? GetOpenSearchIndex(string id) => client.Databases_get_clusterAsync(new Guid(id)).GetAwaiter().GetResult()?.Database;
    #endregion

    #endregion

    #region Fields
    protected DigitalOceanClient client = new DigitalOceanClient();
    #endregion
}
