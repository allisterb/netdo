# Donna JavaScript API

This document is the reference for the Donna JavaScript API. It provides the name of each Donna API method, a description of its purpose, and each parameter's type.
Each method is accessed from the `api` instance which is available in every Donna session.
The return types are provided as JSON schemas to help the agent generate accurate JavaScript code.

## Global Functions

`log(message)`
Outputs an informational message to the Donna console.

`error(message)`
Outputs an error message to the Donna console.

`confirm(message)`
Displays a confirmation dialog with the specified message. Returns `true` if the user confirms, `false` otherwise.

`ask(prompt)`
Prompts the user for a text input with the specified prompt string. Returns the entered string.

`select(title, choices)`
Displays a selection prompt with a title and an array of choice strings. Returns the string selected by the user.

`table(headers, dataRows)`
Displays a formatted table in the console.
- `headers`: An array of strings representing the table column headers.
- `dataRows`: A 2D array of strings, where each inner array represents a row of data.

## Account

`GetAccount()`
Get the current account information. Returns an `AccountResponse` object.

### Account Schema
```json
{
  "type": "object",
  "properties": {
    "account": {
      "type": "object",
      "properties": {
        "droplet_limit": { "type": "integer" },
        "floating_ip_limit": { "type": "integer" },
        "email": { "type": "string" },
        "name": { "type": "string" },
        "uuid": { "type": "string" },
        "email_verified": { "type": "boolean" },
        "status": { "type": "string", "enum": ["active", "warning", "locked"] },
        "status_message": { "type": "string" }
      }
    }
  }
}
```

## Billing

`GetBalance()`
Get the current balance for the account. Returns a `Balance` object.

`ListBillingHistory()`
Get current user account billing history entry. Returns a `BillingHistoryResponse` object.

`ListInvoices()`
Retrieve a list of all invoices for the current customer. Returns an `InvoicesResponse` object.

`GetInvoice(uuid)`
Retrieve an invoice summary by UUID. Returns an `InvoiceSummary` object.

### Balance Schema
```json
{
  "type": "object",
  "properties": {
    "month_to_date_balance": { "type": "string", "description": "Balance as of the generated_at time." },
    "account_balance": { "type": "string", "description": "Current balance of the customer's most recent billing activity." },
    "month_to_date_usage": { "type": "string", "description": "Amount used in the current billing period." },
    "generated_at": { "type": "string", "format": "date-time", "description": "The time at which balances were most recently generated." }
  }
}
```

### BillingHistory Schema
```json
{
  "type": "object",
  "properties": {
    "billing_history": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "description": { "type": "string" },
          "amount": { "type": "string" },
          "invoice_id": { "type": "string" },
          "invoice_uuid": { "type": "string" },
          "date": { "type": "string", "format": "date-time" },
          "type": { "type": "string" }
        }
      }
    },
    "meta": {
      "type": "object",
      "properties": {
        "total": { "type": "integer" }
      }
    }
  }
}
```

### InvoicesResponse Schema
```json
{
  "type": "object",
  "properties": {
    "invoices": {
      "type": "array",
      "items": {
        "type": "object",
        "properties": {
          "invoice_uuid": { "type": "string" },
          "invoice_id": { "type": "string" },
          "amount": { "type": "string" },
          "invoice_period": { "type": "string" },
          "updated_at": { "type": "string" }
        }
      }
    },
    "invoice_preview": {
      "type": "object",
      "properties": {
        "invoice_uuid": { "type": "string" },
        "invoice_id": { "type": "string" },
        "amount": { "type": "string" },
        "invoice_period": { "type": "string" },
        "updated_at": { "type": "string" }
      }
    },
    "meta": {
      "type": "object",
      "properties": {
        "total": { "type": "integer" }
      }
    }
  }
}
```

### InvoiceSummary Schema
```json
{
  "type": "object",
  "properties": {
    "invoice_uuid": { "type": "string" },
    "invoice_id": { "type": "string" },
    "billing_period": { "type": "string" },
    "amount": { "type": "string" },
    "user_name": { "type": "string" },
    "user_company": { "type": "string" },
    "user_email": { "type": "string" }
  }
}
```

## Gradient AI Agent Management

`ListAgents()`
Lists all agents for the account. Returns an array of `ApiAgentPublic` objects.

`GetAgent(uuid)`
Get the agent with a specific uuid. Returns an `ApiAgent` object.

`ListModels()`
List available AI models. Returns an array of `ApiModelPublic` objects.

`ListWorkspaces()`
List available GenAI workspaces. Returns an array of `ApiWorkspace` objects.

### ApiAgentPublic Schema
```json
{
  "type": "object",
  "properties": {
    "uuid": { "type": "string" },
    "name": { "type": "string" },
    "description": { "type": "string" },
    "instruction": { "type": "string" },
    "project_id": { "type": "string" },
    "region": { "type": "string" },
    "model": { "type": "object" },
    "url": { "type": "string" },
    "created_at": { "type": "string", "format": "date-time" },
    "updated_at": { "type": "string", "format": "date-time" },
    "tags": { "type": "array", "items": { "type": "string" } }
  }
}
```

### ApiAgent Schema
The `ApiAgent` object includes all properties of `ApiAgentPublic` plus additional configuration:
```json
{
  "type": "object",
  "properties": {
    "uuid": { "type": "string" },
    "name": { "type": "string" },
    "functions": { "type": "array", "items": { "type": "object" } },
    "guardrails": { "type": "array", "items": { "type": "object" } },
    "knowledge_bases": { "type": "array", "items": { "type": "object" } },
    "workspace": { "$ref": "#/definitions/ApiWorkspace" }
  }
}
```

### ApiModelPublic Schema
```json
{
  "type": "object",
  "properties": {
    "uuid": { "type": "string" },
    "id": { "type": "string", "description": "Human-readable model identifier" },
    "name": { "type": "string" },
    "is_foundational": { "type": "boolean" },
    "version": { "type": "object" }
  }
}
```

### ApiWorkspace Schema
```json
{
  "type": "object",
  "properties": {
    "uuid": { "type": "string" },
    "name": { "type": "string" },
    "description": { "type": "string" },
    "agents": { "type": "array", "items": { "type": "object" } },
    "created_at": { "type": "string", "format": "date-time" }
  }
}
```

## Apps

`ListApps()`
List all apps. Returns an array of `App` objects.

`GetAppById(appid)`
Retrieve an existing app by its ID. Returns an `App` object.

`GetAppHealth(appid)`
Retrieve the health status of an app. Returns an `App_health` object.

### App Schema
```json
{
  "type": "object",
  "properties": {
    "active_deployment": { "type": "object" },
    "created_at": { "type": "string", "format": "date-time" },
    "default_ingress": { "type": "string" },
    "domains": { "type": "array", "items": { "type": "object" } },
    "id": { "type": "string" },
    "in_progress_deployment": { "type": "object" },
    "last_deployment_created_at": { "type": "string", "format": "date-time" },
    "live_domain": { "type": "string" },
    "live_url": { "type": "string" },
    "live_url_base": { "type": "string" },
    "owner_uuid": { "type": "string" },
    "pending_deployment": { "type": "object" },
    "project_id": { "type": "string" },
    "region": { "type": "object" },
    "spec": { "type": "object" },
    "tier_slug": { "type": "string" },
    "updated_at": { "type": "string", "format": "date-time" }
  }
}
```

### AppHealth Schema
```json
{
  "type": "object",
  "properties": {
    "components": { "type": "array", "items": { "type": "object" } },
    "functions_components": { "type": "array", "items": { "type": "object" } }
  }
}
```

## Databases

`ListDatabaseClusters()`
List all database clusters. Returns an array of `Database_cluster_read` objects.

`ListOpenSearchIndexes(guid)`
List indexes for an OpenSearch cluster. Returns an array of `Opensearch_index` objects.

`GetOpenSearchIndex(guid)`
Retrieve an existing database cluster (named as GetOpenSearchIndex). Returns a `Database_cluster_read` object.

`UpdateDatabaseClusterRegion(guid, region)`
Migrate a database cluster to a new region. Returns nothing.

### DatabaseClusterRead Schema
```json
{
  "type": "object",
  "properties": {
    "id": { "type": "string", "format": "uuid" },
    "name": { "type": "string" },
    "engine": { "type": "string" },
    "version": { "type": "string" },
    "semantic_version": { "type": "string" },
    "num_nodes": { "type": "integer" },
    "size": { "type": "string" },
    "region": { "type": "string" },
    "status": { "type": "string" },
    "created_at": { "type": "string", "format": "date-time" },
    "private_network_uuid": { "type": "string" },
    "tags": { "type": "array", "items": { "type": "string" } },
    "db_names": { "type": "array", "items": { "type": "string" } },
    "connection": { "type": "object" },
    "private_connection": { "type": "object" },
    "standby_connection": { "type": "object" },
    "standby_private_connection": { "type": "object" },
    "users": { "type": "array", "items": { "type": "object" } },
    "maintenance_window": { "type": "object" },
    "project_id": { "type": "string", "format": "uuid" },
    "rules": { "type": "array", "items": { "type": "object" } },
    "storage_size_mib": { "type": "integer" }
  }
}
```

### OpensearchIndex Schema
```json
{
  "type": "object",
  "properties": {
    "index_name": { "type": "string" },
    "number_of_shards": { "type": "integer" },
    "number_of_replicas": { "type": "integer" },
    "size": { "type": "integer" },
    "created_time": { "type": "string", "format": "date-time" },
    "status": { "type": "string" },
    "health": { "type": "string" }
  }
}
```

## Project Management

`ListProjects()`
List all projects. Returns an array of `Project` objects.

### Project Schema
```json
{
  "type": "object",
  "properties": {
    "id": { "type": "string", "format": "uuid" },
    "name": { "type": "string" },
    "description": { "type": "string" },
    "purpose": { "type": "string" },
    "environment": { "type": "string" },
    "is_default": { "type": "boolean" },
    "created_at": { "type": "string", "format": "date-time" }
  }
}
```
