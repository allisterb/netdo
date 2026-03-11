namespace NetDo.Cli;

using System;
using System.Collections.Generic;
using System.Text;

using Jint;

public class JSInterop
{
    public static void Execute(string src)
    {
        var jsoptions = new Options();
        jsoptions.Host.StringCompilationAllowed = false;
        
        var engine = new Engine(jsoptions);
        engine.Execute(src);
        engine.
    }
}
