namespace DigitalOcean.Cli;

using Spectre.Console;
using System;
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

    /// <summary>
    /// List Knowledge Bases
    /// </summary>
    public ApiKnowledgeBase[]? ListKnowledgeBases() => client.Genai_list_knowledge_basesAsync(null, null).GetAwaiter().GetResult()?.Knowledge_bases?.ToArray();

    /// <summary>
    /// Retrieve information about an existing knowledge base
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public ApiKnowledgeBase? GetKnowledgeBase(string uuid) => client.Genai_get_knowledge_baseAsync(uuid).GetAwaiter().GetResult()?.Knowledge_base;

    /// <summary>
    /// Detach a knowledge base from an agent
    /// </summary>
    /// <param name="agentuuiid">UUID of agent</param>
    /// <param name="kbuuid">UUID of knowledge base</param>
    /// <returns></returns>
    public void DetachKnowledgeBaseFromAgent(string agentuuiid, string kbuuid) =>
        Confirm(() => client.Genai_detach_knowledge_baseAsync(agentuuiid, kbuuid).GetAwaiter().GetResult().Agent, $"Detach the knowledge base {kbuuid} from agent {agentuuiid}");
    #endregion

    #region Apps
    /// <summary>
    /// List All Apps
    /// </summary>
    /// <returns></returns>
    public App[]? ListApps() => client.Apps_listAsync(null, null, null).GetAwaiter().GetResult()?.Apps?.ToArray();

    /// <summary>
    /// Retrieve an existing app by app id
    /// </summary>
    /// <param name="appid"></param>
    /// <returns></returns>
    public App? GetAppById(string appid) => client.Apps_getAsync(appid, null).GetAwaiter().GetResult().App;
   
    /// <summary>
    /// Retrieve App Health
    /// </summary>
    /// <param name="appid"></param>
    /// <returns></returns>
    public App_health? GetAppHealth(string appid) => client.Apps_get_healthAsync(appid).GetAwaiter().GetResult().App_health;
    #endregion

    #region Databases
    /// <summary>
    /// List All Database Clusters
    /// </summary>
    /// <returns></returns>
    public Database_cluster_read[]? ListDatabaseClusters() => client.Databases_list_clustersAsync(null).GetAwaiter().GetResult().Databases?.ToArray();
    
    /// <summary>
    /// List Indexes for an OpenSearch cluster
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public Opensearch_index[]? ListOpenSearchIndexes(string guid) => client.Databases_list_opeasearch_indexesAsync(new Guid(guid)).GetAwaiter().GetResult()?.Indexes?.ToArray();

    /// <summary>
    /// Retrieve an existing database cluster
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public Database_cluster_read? GetOpenSearchIndex(string guid) => client.Databases_get_clusterAsync(new Guid(guid)).GetAwaiter().GetResult()?.Database;

    /// <summary>
    /// Migrate a database cluster to a new region
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="region"></param>
    public void UpdateDatabaseClusterRegion(string guid, string region) => Confirm(() => client.Databases_update_regionAsync(new Guid(guid), new Body5(region)).GetAwaiter().GetResult(), 
        $"Migrate database cluster {guid} to region {region}.");
    #endregion

    #region General
    protected static void Confirm(System.Action method, string message)
    {
        if (JSInterp.ConsoleConfirm("[red] Warning! This operation can potentially cause data loss. Confirm the following operation:[/] " + message))
        {
            method.Invoke();
            AnsiConsole.WriteLine("Operation complete.");
        }
    }

    protected static T? Confirm<T>(Func<T> method, string message)
    {
        if (JSInterp.ConsoleConfirm("[red] Warning! This operation can potentially cause data loss. Confirm the following operation:[/] " + message))
        {
            return method.Invoke();
        }
        else
        {
            return default;
        }
    }
    #endregion

    #endregion

    #region Fields
    protected DigitalOceanClient client = new DigitalOceanClient();
    #endregion
}
