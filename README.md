## About
netdo is a set of libraries that makes DigitalOcean IaaS and PaaS management and Gradient AI services available to .NET developers. 
netdo provides C# bindings for the DigitalOcean OpenAPIv3 HTTP API as well as a higher-level library for integrating Gradient AI sevices with .NET agent libraries and frameworks like Microsft.Extensions.AI and the [Microsoft Agent Framework](https://github.com/microsoft/agent-framework). Microsoft Agent Framework via netdo allows .NET developers to develop agentic AI applications using Gradient AI hosted agents and knowledge bases and observability and other features

## Building

Clone the repository and run `build.cmd` or `./build` from the repository root. This will build the netdo libraries and the CLI.

## Getting started
* Set your DIGITALOCEAN_API_TOKEN environment variable to the token you want to use with netdo.
* Run `netdo.cmd` or `./netdo` to see the available CLI commands. E.g to list all agents on the agent platfrom run `./netdo agents list`

## Donna
Donna requires an agent and knowledge base hosted on Gradient AI. You can use the netdo CLI to create a Donna agent using the prompt in `docs/prompts.md`:

`./netdo agents --create --name donna1 --instructions @docs/prompts.md --model_uuid <mymodeluuid> --project_uuid <myprojuuid> --workspace_uuid <myworkspaceuuid> --region <myregion>`

You can also use the CLI to create the knowledge base

`./netdo kb --create --name <mykbname> --region <mykbregion> --project-id <myknprojectid> --embedding-model-uuid <mykbmodelid>`

and add a bucket with the Donna spec in docs/Donna.md as a data source:
`./netdo kb --add-datasource --uuid <kbuuid>  --bucket-name <name> --region <region>`

See the CLI docs for more info if you want to use it, or alternatively you can use the Digital Ocean web interface.

Once you have the agent and kb, create an agent API key and set the GRADIENT_AGENT_API_KEY environment variable to the key value. Then you can run

`./netdo donna --agentuuid <donna_agent_uuid>`

to start the Donna agent.
