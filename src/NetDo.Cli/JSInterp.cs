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
        Info(o?.ToString() ?? "");
        switch (o)
        {
            case string str:
                AnsiConsole.WriteLine(str);
                break;

            default:
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
                AnsiConsole.Console.WriteJson(json);
                break;
        }
        AnsiConsole.WriteLine(Environment.NewLine);
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
            .SetValue("confirm", Confirm)
            .SetValue("ask", Ask)
            .SetValue("select", Select)
            .SetValue("table", DrawTable)
            .SetValue("api", new DonnaApi());
        engine.Execute(src);
    }

    public static bool Confirm(string msg) => AnsiConsole.Confirm(msg);

    public static string Ask(string prompt) => AnsiConsole.Ask<string>(prompt);

    public static string Select(string title, params string[] choices) => AnsiConsole.Prompt(new SelectionPrompt<string>().Title(title).AddChoices(choices));

    public static void DrawTable(string[] headers, string[][] dataRows)
    {
        var table = new Table();
        var headerStyle = new Style(foreground: Color.White, decoration: Decoration.Bold | Decoration.Underline);
        foreach (var header in headers)
        {
            table.AddColumn(new TableColumn(new Markup(header, headerStyle)).Alignment(Justify.Center));
        }
        foreach (var row in dataRows)
        {
            if (row.Length != headers.Length)
            {
                AnsiConsole.MarkupLine($"[yellow]Warning: Skipping a row due to column count mismatch.[/]");
                continue;
            }
            table.AddRow(row);
        }

        AnsiConsole.Write(table);
    }
}
