namespace DigitalOcean.Tests.Gradient;

using System;
using System.Collections.Generic;
using System.Text;

using DigitalOcean.Cli;

public class DonnaTests : TestsRuntime
{
    [Fact]
    public void CanExecuteJavaScript()
    {
        var js =
            """
            var a = api.ListAgents();
            log(a[0].Uuid);
            """;
        
        JSInterp.ConsoleExecute(js);
    }

    [Fact]
    public void CanExtractJavaScript()
    {
        var js =
            """
                        ```javascript
            (async () => {
              try {
                /* Fetch all agents */
                const agents = await api.ListAgents();

                /* Filter agents in the tor1 region */
                const torAgents = agents.filter(a => a.region === 'tor1');

                /* Output the filtered list */
                log(JSON.stringify(torAgents, null, 2));
              } catch (err) {
                error(`Failed to list agents: ${err.message}`);
              }
            })();
            ```
            """;
        var j = JSInterp.ExtractJSFromMarkdown(js);
        Assert.NotEmpty(j);  

    }
}
