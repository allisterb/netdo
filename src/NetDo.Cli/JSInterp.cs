namespace DigitalOcean.Cli;

using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Jint;
using Spectre.Console;
using NTokenizers.Extensions.Spectre.Console;
using NTokenizers.Extensions.Spectre.Console.Styles;

public class JSInterp : Runtime
{
    protected readonly static Markup infoHeader = new Markup(Markup.Escape("info"), new Style(foreground: Color.White, background: Color.LightGreen));

    protected readonly static Style infoStyle = new Style(foreground: Color.White);

    protected static string LogTime => "[" + DateTime.Now.ToLongTimeString() + "] ";

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
        var s = o.ToString() ?? "";
        Info(s);
        AnsiConsole.Write(LogTime);
        AnsiConsole.Write(infoHeader);
        AnsiConsole.Write(" ");
        WriteObject(o);
    }

    public static void WriteObject(object o)
    {
        switch (o)
        {
            case string s:
                AnsiConsole.WriteLine(s);
                break;

            default:
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
                AnsiConsole.Console.WriteJson(json, JsonStyles.Default);
                break;
        }
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
