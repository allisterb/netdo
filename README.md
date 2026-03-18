## About
netdo is a set of libraries that makes DigitalOcean IaaS and PaaS management and Gradient AI services available to .NET developers. 
netdo provides C# bindings for the DigitalOcean OpenAPIv3 HTTP API as well as a higher-level library for integrating Gradient AI sevices with .NET agent libraries and frameworks like Microsft.Extensions.AI and the [Microsoft Agent Framework]([Microsoft Agent Framework](https://github.com/microsoft/agent-framework). Microsoft Agent Framework via netdo allows .NET developers to develop agentic AI applications using Gradient AI hosted agents and knowledge bases and observability and other features

## Building

Clone the repository and run `build.cmd` or `./build` from the repository root. This will build the netdo libraries and the CLI

## Getting started
* Set your DIGITALOCEAN_API_TOKEN environment variable to the token you want to use with netdo.
* Run `netdo.cmd` or `./netdo` to see the available CLI commands. E.g to list all agents on the agent platfrom run `./netdo agents list`
