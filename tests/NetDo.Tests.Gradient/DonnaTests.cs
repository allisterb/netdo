namespace DigitalOcean.Tests.Gradient;

using System;
using System.Collections.Generic;
using System.Text;

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
        
        Cli.JSInterp.Execute(js);
    }
}
