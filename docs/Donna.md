# Donna JavaScript API

This document is the reference for the Donna JavaScript API. It provides the name of each Donna API method, a description of its purpose, and each parameter's type.
Each method is accessed from the `api` instance which is available in every Donna session.
The return types are provided as JSON schemas to help the agent generate accurate JavaScript code.

## Global Functions

`log(message)`
Outputs an informational message to the Donna console.

`error(message)`
Outputs an error message to the Donna console.

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
