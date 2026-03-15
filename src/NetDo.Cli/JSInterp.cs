namespace DigitalOcean.Cli;

using System;

using Jint;
using Spectre.Console;

public class JSInterp : Runtime
{
    protected readonly static Markup infoHeader = new Markup("[info]", new Style(foreground: Color.LightGreen));

    protected readonly static Style infoStyle = new Style(foreground: Color.White);

    public static void JSInfo(object o)
    {
        var s = o.ToString() ?? "";
        Info(s);
        AnsiConsole.Write(infoHeader);
        AnsiConsole.Write(new Markup(s + Environment.NewLine, infoStyle));
    }

    public static void JSError(object o)
    {
        var s = o.ToString() + Environment.NewLine;
        Error(s);
        AnsiConsole.Write(new Markup(s, new Style(foreground: Color.Red)));
    }

    public static void Execute(string src)
    {
        var jsoptions = new Jint.Options();
        jsoptions.Host.StringCompilationAllowed = false;

        var engine = new Engine(jsoptions)
            .SetValue("log", JSInfo)
            .SetValue("error", JSError)
            .SetValue("api", new DonnaApi());
        engine.Execute(src);
    }
}
