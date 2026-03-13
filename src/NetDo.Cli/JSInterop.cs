namespace DigitalOcean.Cli;

using System;

using Jint;
using Spectre.Console;

public class JSInterop : Runtime
{    
    public static void JSInfo(object o)
    {
        var s = o.ToString() + Environment.NewLine;
        Info(s);
        AnsiConsole.Write(new Markup(s, new Style(foreground: Color.White)));
    }
    
    public static void Execute(string src)
    {
        var jsoptions = new Jint.Options();
        jsoptions.Host.StringCompilationAllowed = false;

        var engine = new Engine(jsoptions)
            .SetValue("log", JSInfo)
            .SetValue("api", new DonnaApi());
        engine.Execute(src);
    }
}
