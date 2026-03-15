namespace DigitalOcean.Cli;

using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Jint;
using Spectre.Console;

public class JSInterp : Runtime
{    
    public static string[] ExtractJSFromMarkdown(string src)
    {
        var matches = Regex.Matches(src, @"```javascript\s+(.*?)\s+```", RegexOptions.Singleline);
        var list = new List<string>();
        foreach (Match match in matches)
        {
            list.Add(match.Groups[1].Value);
        }
        return list.ToArray();
    }

    public static void JSInfo(object o)
    {
        var s = o.ToString() + Environment.NewLine;
        Info(s);
        AnsiConsole.Write(new Markup(s, new Style(foreground: Color.White)));
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
