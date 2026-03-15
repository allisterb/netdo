# Donna JavaScript API

This document is the reference for the Donna JavaScript API. It provides the name of each Donna API method, a description of its purpose, and each parameter's type.
Each method is accessed from the `api` instance which is available in every Donna session.
The return types are provided as JSON schemas to help the agent generate accurate JavaScript code.

## Global Functions

`log(message)`
Outputs an informational message to the Donna console.

`error(message)`
Outputs an error message to the Donna console.

## Billing

`GetBalance()`
Get the current balance for the account. Returns a `Balance` object.

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
